
// Use Parse.Cloud.define to define as many cloud functions as you want.
// For example:
Parse.Cloud.define("hello", function(request, response) {
  response.success("Hello world!");
});


var Tag = Parse.Object.extend("Tag");
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
