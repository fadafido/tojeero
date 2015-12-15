Parse.Cloud.define("hello", function(request, response) {
  response.success("Hello world!");
});
 
Parse.Cloud.beforeSave("Tag", function(request, response) {
  if (!request.object.get("text")) {
    response.error('A tag must have a non empty text.');
  } else {
    var query = new Parse.Query(Tag);
    var tag = request.object;
    var tagText = tag.get("text");
    tagText = tagText.toLowerCase();
    tag.set("text", tagText);
    query.equalTo("text", tagText);
    query.first({
      success: function(object) {
        if (object) {
          response.error("A tag with this text already exists.");
        } else {
          response.success();
        }
      },
      error: function(error) {
        response.error("Could not validate uniqueness for this tag object.");
      }
    });
  }
});
 
var algoliasearch = require('cloud/algoliasearch.parse.js');
var client = algoliasearch('I72QVLIB9D', '1bbda88eb37f712280d29416e5278d59');
var storeIndex = client.initIndex('Stores');
var productIndex = client.initIndex('Products');
  
Parse.Cloud.define("uploadStoresToAlgolia", function(request, response) {
    indexClass('Store', storeIndex, response, getStoreObjectForAlgolia);
});
 
Parse.Cloud.define("uploadProductsToAlgolia", function(request, response) {
    indexClass('StoreItem', productIndex, response, getProductObjectForAlgolia);
});

Parse.Cloud.afterSave('Store', function(request) {
  saveObjectToIndex(storeIndex, request, getStoreObjectForAlgolia);
});


Parse.Cloud.afterSave('StoreItem', function(request) {
  saveObjectToIndex(productIndex, request, getProductObjectForAlgolia);
});

Parse.Cloud.afterDelete('Store', function(request) {
  deleteObjectFromIndex(storeIndex, request)
});

Parse.Cloud.afterDelete('StoreItem', function(request) {
  deleteObjectFromIndex(productIndex, request)
});
 
function indexClass(className, classIndex, response, getObjectForAlgolia){
    var objectsToIndex = [];
  
    //Create a new query for Objects for class name
    var query = new Parse.Query(className);
    query.limit(1000);
  
    // Find all items
  query.find({
    success: function(Objects) {
      // prepare objects to index from Objects
      objectsToIndex = Objects.map(function(object) {
        
        var objectToSave = getObjectForAlgolia(object)
        return objectToSave;
      });
  
      // Add or update new objects
      classIndex.saveObjects(objectsToIndex, function(err, content) {
        if (err) {
          throw err;
        }else{
            response.success("Parse<>Algolia "+className+" import done'");
        }
      });
    },
    error: function(err) {
      throw err;
    }
  });
}

function saveObjectToIndex(algoliaIndex, request, getObjectForAlgolia)
{
  var objectToSave = getObjectForAlgolia(request.object);

  // Add or update object
  algoliaIndex.saveObject(objectToSave, function(err, content) {
    if (err) {
      throw err;
    }

    console.log('Parse<>Algolia object saved');
  });
}

function deleteObjectFromIndex(algoliaIndex, request)
{
  // Get Algolia objectID
  var objectID = request.object.id;

  // Remove the object from Algolia
  algoliaIndex.deleteObject(objectID, function(err, content) {
    if (err) {
      throw err;
    }

    console.log('Parse<>Algolia object deleted');
  });
}

function getStoreObjectForAlgolia(store)
{
  var jsonStore = store.toJSON();
  
  jsonStore.objectID = jsonStore.objectId;
  jsonStore.imageUrl = jsonStore.image != null ? jsonStore.image.url : null;
  jsonStore.categoryID = jsonStore.category != null ? jsonStore.category.objectId : null;
  jsonStore.countryID = jsonStore.country != null ? jsonStore.country.objectId : null;
  jsonStore.cityID = jsonStore.city != null ? jsonStore.city.objectId : null;
  jsonStore.ownerID = jsonStore.owner != null ? jsonStore.owner.objectId : null;

  delete jsonStore.city;
  delete jsonStore.country;
  delete jsonStore.category;
  delete jsonStore.image;
  delete jsonStore.searchTokens;
  delete jsonStore.owner;
  delete jsonStore.products;

  return jsonStore;
}

function getProductObjectForAlgolia(product)
{
  var jsonProduct = product.toJSON();

  jsonProduct.objectID = jsonProduct.objectId;
  jsonProduct.imageUrl = jsonProduct.image != null ? jsonProduct.image.url : null;
  jsonProduct.storeID = jsonProduct.store != null ? jsonProduct.store.objectId : null;
  jsonProduct.categoryID = jsonProduct.category != null ? jsonProduct.category.objectId : null;   
  jsonProduct.subcategoryID = jsonProduct.subcategory != null ? jsonProduct.subcategory.objectId : null;
  jsonProduct.countryID =  jsonProduct.country != null ? jsonProduct.country.objectId : null;
  jsonProduct.cityID = jsonProduct.city != null ? jsonProduct.city.objectId : null;
  jsonProduct._tags = jsonProduct.tags;

  delete jsonProduct.store;
  delete jsonProduct.city;
  delete jsonProduct.country;
  delete jsonProduct.category;
  delete jsonProduct.subcategory;
  delete jsonProduct.image;
  delete jsonProduct.searchTokens;
  delete jsonProduct.images;
  delete jsonProduct.tags;
  
  return jsonProduct;
}