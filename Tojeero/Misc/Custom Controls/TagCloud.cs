using System;
using System.Linq;
using System.Collections.ObjectModel;
using Tojeero.Core;
using Xamarin.Forms;
using System.Collections.Specialized;
using Tojeero.Core.Toolbox;

namespace Tojeero.Forms
{
	public class TagCloud : WrapLayout
	{
		#region Private properties and fields

		private ObservedCollection<string> _tags;
		private Button _addButton;
		private NavigationPage _tagSelector;
		private NavigationPage TagSelector
		{
			get
			{
				if (_tagSelector == null)
				{
					var tags = new TagsPage();
					tags.DidClose += tagSelected;
					_tagSelector = new NavigationPage(tags);
				}
				return _tagSelector;
			}
		}
			
		#endregion

		#region Constructors

		public TagCloud()
		{
			this.Orientation = StackOrientation.Horizontal;
			_addButton = new Button()
				{ 
					BackgroundColor = Colors.Green,
					Text = "+",
					TextColor = Color.White,
					FontAttributes = FontAttributes.Bold,
					HeightRequest = 30,
					WidthRequest = 30,
					BorderRadius = 15,
					FontSize=24
				};
			_addButton.Clicked += addButtonClicked;
		}

		#endregion

		#region Properties

		#region Tags

		public static BindableProperty TagsProperty = BindableProperty.Create<TagCloud, ObservableCollection<string>>(o => o.Tags, null, propertyChanged: OnTagsChanged);

		public ObservableCollection<string> Tags
		{
			get { return (ObservableCollection<string>)GetValue(TagsProperty); }
			set { SetValue(TagsProperty, value); }
		}

		private static void OnTagsChanged(BindableObject bindable, ObservableCollection<string> oldvalue, ObservableCollection<string> newvalue)
		{
			var control = (TagCloud)bindable;

			control.disconnectEvents();
			control._tags = new ObservedCollection<string>(newvalue);
			control.clear();
			if (newvalue != null)
				control.Children.InsertRange(0, newvalue.Select(t => control.create(t)));
			control.connectEvents();
		}


		#endregion

		#region Tag view

		public DataTemplate DataTemplate { get; set; }

		#endregion

		#endregion

		#region Utility methods

		private void connectEvents()
		{
			if (_tags != null)
			{
				_tags.OnItemAdded += itemAdded;
				_tags.OnItemMoved += itemMoved;
				_tags.OnItemRemoved += itemRemoved;
				_tags.OnItemReplaced += itemReplaced;
				_tags.OnCleared += cleared;
			}
		}

		private void disconnectEvents()
		{
			if (_tags != null)
			{
				_tags.OnItemAdded -= itemAdded;
				_tags.OnItemMoved -= itemMoved;
				_tags.OnItemRemoved -= itemRemoved;
				_tags.OnItemReplaced -= itemReplaced;
				_tags.OnCleared -= cleared;
			}
		}

		private void itemAdded(ObservableCollection<string> sender, int index, string tag)
		{
			this.Children.Insert(index, create(tag));
		}

		private void itemMoved(ObservableCollection<string> sender, int oldIndex, int newIndex, string tag)
		{
			var oldItem = this.Children[oldIndex];
			this.Children.Insert(newIndex, create(tag));
			this.Children.Remove(oldItem);
		}

		private void itemRemoved(ObservableCollection<string> sender, int index, string tag)
		{
			this.Children.RemoveAt(index);
		}

		private void itemReplaced(ObservableCollection<string> sender, int index, string oldTag, string newTag)
		{
			var tag = this.Children[index];
			tag.BindingContext = newTag;
		}

		private void cleared(ObservableCollection<string> sender)
		{
			this.clear();
		}

		private TagControl create(string tag)
		{
			var tagControl = new TagControl();
			tagControl.BindingContext = tag;
			tagControl.DeleteTagAction = (t) =>
			{
				var index = this.Children.IndexOf(tagControl);
				if (index >= 0)
					this._tags.Source.RemoveAt(index);
			};
			return tagControl;
		}

		private void clear()
		{
			Children.Clear();
			Children.Add(_addButton);
		}

		private async void addButtonClicked (object sender, EventArgs e)
		{
			await this.Navigation.PushModalAsync(this.TagSelector);
		}

		private void tagSelected (object sender, EventArgs<ITag> e)
		{
			if (e.Data != null && !this.Tags.Contains(e.Data.Text))
			{
				this.Tags.Add(e.Data.Text);
			}
		}

		#endregion

	}
}

