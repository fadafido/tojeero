using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tojeero.Core;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Views.Tag;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public partial class TagCloud : StackLayout
    {
        #region Private properties and fields

        private ObservedCollection<string> _tags;

        private TagsPage _tagsPage;
        private NavigationPage _tagSelector;

        private NavigationPage TagSelector
        {
            get
            {
                if (_tagSelector == null)
                {
                    _tagsPage = new TagsPage();
                    _tagsPage.DidClose += tagSelected;
                    _tagsPage.ViewModel.EnableTagCreation = EnableTagCreation;
                    _tagSelector = new NavigationPage(_tagsPage);
                }
                return _tagSelector;
            }
        }

        #endregion

        #region Constructors

        public TagCloud()
        {
            InitializeComponent();
            wrapLayout.Orientation = StackOrientation.Horizontal;
        }

        #endregion

        #region Properties

        #region Tags

        public static BindableProperty TagsProperty =
            BindableProperty.Create<TagCloud, ObservableCollection<string>>(o => o.Tags, null,
                propertyChanged: OnTagsChanged);

        public ObservableCollection<string> Tags
        {
            get { return (ObservableCollection<string>) GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        private static void OnTagsChanged(BindableObject bindable, ObservableCollection<string> oldvalue,
            ObservableCollection<string> newvalue)
        {
            var control = (TagCloud) bindable;

            control.disconnectEvents();
            control._tags = new ObservedCollection<string>(newvalue);
            control.clear();
            if (newvalue != null)
                control.TagControls.InsertRange(0, newvalue.Select(t => control.create(t)));
            control.connectEvents();
        }

        #endregion

        #region EnableTagCreation

        public static BindableProperty EnableTagCreationProperty =
            BindableProperty.Create<TagCloud, bool>(o => o.EnableTagCreation, false,
                propertyChanged: OnEnableTagCreationChanged);

        public bool EnableTagCreation
        {
            get { return (bool) GetValue(EnableTagCreationProperty); }
            set { SetValue(EnableTagCreationProperty, value); }
        }

        private static void OnEnableTagCreationChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var tagCloud = bindable as TagCloud;
            if (tagCloud._tagsPage != null)
                tagCloud._tagsPage.ViewModel.EnableTagCreation = newvalue;
        }

        #endregion

        #region Tag view

        public DataTemplate DataTemplate { get; set; }

        #endregion

        #endregion

        #region Utility methods

        private IList<View> TagControls
        {
            get { return wrapLayout.Children; }
        }

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
            TagControls.Insert(index, create(tag));
        }

        private void itemMoved(ObservableCollection<string> sender, int oldIndex, int newIndex, string tag)
        {
            var oldItem = TagControls[oldIndex];
            TagControls.Insert(newIndex, create(tag));
            TagControls.Remove(oldItem);
        }

        private void itemRemoved(ObservableCollection<string> sender, int index, string tag)
        {
            TagControls.RemoveAt(index);
        }

        private void itemReplaced(ObservableCollection<string> sender, int index, string oldTag, string newTag)
        {
            var tag = TagControls[index];
            tag.BindingContext = newTag;
        }

        private void cleared(ObservableCollection<string> sender)
        {
            clear();
        }

        private TagControl create(string tag)
        {
            var tagControl = new TagControl();
            tagControl.BindingContext = tag;
            tagControl.DeleteTagAction = t =>
            {
                var index = TagControls.IndexOf(tagControl);
                if (index >= 0)
                    _tags.Source.RemoveAt(index);
            };
            return tagControl;
        }

        private void clear()
        {
            TagControls.Clear();
        }

        private async void addButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(TagSelector);
        }

        private void tagSelected(object sender, EventArgs<string> e)
        {
            if (e.Data != null && !Tags.Contains(e.Data))
            {
                Tags.Add(e.Data);
            }
        }

        #endregion
    }
}