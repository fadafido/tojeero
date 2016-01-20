using System;
using System.Linq;
using System.Collections.ObjectModel;
using Tojeero.Core;
using Xamarin.Forms;
using System.Collections.Specialized;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;
using System.Collections.Generic;
using Tojeero.Forms.Toolbox;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tojeero.Forms
{
	public partial class MultiImageControl : StackLayout
	{
		#region Private properties and fields

		private ObservedCollection<IImageViewModel> _images;
		private int MaxCount { get; set; }
		AsyncReaderWriterLock _removeLocker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public MultiImageControl()
		{
			InitializeComponent();
			MaxCount = 7;
			this.wrapLayout.Orientation = StackOrientation.Horizontal;
			this.wrapLayout.Spacing = 1;
		}
			
		#endregion

		#region Properties

		public Func<IImageViewModel> ImageFactory { get; set; }

		#region Images

		public static BindableProperty ImagesProperty = BindableProperty.Create<MultiImageControl, ObservableCollection<IImageViewModel>>(o => o.Images, null, propertyChanged: OnImagesChanged);

		public ObservableCollection<IImageViewModel> Images
		{
			get { return (ObservableCollection<IImageViewModel>)GetValue(ImagesProperty); }
			set { SetValue(ImagesProperty, value); }
		}

		private static void OnImagesChanged(BindableObject bindable, ObservableCollection<IImageViewModel> oldvalue, ObservableCollection<IImageViewModel> newvalue)
		{
			var control = (MultiImageControl)bindable;

			control.disconnectEvents();
			control._images = new ObservedCollection<IImageViewModel>(newvalue);
			control.clear();
			if (newvalue != null)
				control.ImageControls.InsertRange(0, newvalue.Select(t => control.create(t)));
			control.connectEvents();
		}


		#endregion

		#endregion

		#region Utility methods

		private new IList<View> ImageControls
		{
			get
			{
				return this.wrapLayout.Children;
			}
		}

		private async void addImageButtonTapped(object sender, EventArgs args)
		{
			var parent = this.FindParent<Page>();
			var image = await ImageToolbox.PickImage(parent);
			if (image != null)
			{
				if (ImageFactory == null)
				{
					throw new NullReferenceException("MultiImageControl should have non-null ImageFactory.");
				}
				var imageViewModel = ImageFactory();
				imageViewModel.NewImage = image;
				if(this._images == null || this._images.Source == null)
				{
					throw new NullReferenceException("MultiImageControl should have non-empty binding for Images observable collection.");
				}
				this._images.Source.Add(imageViewModel);
			}
		}

		private void connectEvents()
		{
			if (_images != null)
			{
				_images.OnItemAdded += itemAdded;
				_images.OnItemMoved += itemMoved;
				_images.OnItemRemoved += itemRemoved;
				_images.OnItemReplaced += itemReplaced;
				_images.OnCleared += cleared;
			}
		}

		private void disconnectEvents()
		{
			if (_images != null)
			{
				_images.OnItemAdded -= itemAdded;
				_images.OnItemMoved -= itemMoved;
				_images.OnItemRemoved -= itemRemoved;
				_images.OnItemReplaced -= itemReplaced;
				_images.OnCleared -= cleared;
			}
		}

		private void itemAdded(ObservableCollection<IImageViewModel> sender, int index, IImageViewModel image)
		{
			this.ImageControls.Insert(index, create(image));
			this.checkCount();
		}

		private void itemMoved(ObservableCollection<IImageViewModel> sender, int oldIndex, int newIndex, IImageViewModel image)
		{
			var oldItem = this.ImageControls[oldIndex];
			this.ImageControls.Insert(newIndex, create(image));
			this.ImageControls.Remove(oldItem);
		}

		private void itemRemoved(ObservableCollection<IImageViewModel> sender, int index, IImageViewModel image)
		{
			this.ImageControls.RemoveAt(index);
			this.checkCount();
		}

		private void itemReplaced(ObservableCollection<IImageViewModel> sender, int index, IImageViewModel oldImage, IImageViewModel newImage)
		{
			var imageControl = this.ImageControls[index] as ImageControl;
			imageControl.ViewModel = newImage;
		}

		private void cleared(ObservableCollection<IImageViewModel> sender)
		{
			this.clear();
			this.checkCount();
		}

		private ImageControl create(IImageViewModel image)
		{
			var imageControl = new ImageControl();
			imageControl.HeightRequest = 95;
			imageControl.WidthRequest = 95;
			imageControl.ViewModel = image;
			
			return imageControl;
		}

		private void clear()
		{
			ImageControls.Clear();
		}

		private void checkCount()
		{
			if (this.Images != null && this.Images.Count >= MaxCount)
				this.addImageButton.IsVisible = false;
			else
				this.addImageButton.IsVisible = true;
		}

		#endregion
	}
}

