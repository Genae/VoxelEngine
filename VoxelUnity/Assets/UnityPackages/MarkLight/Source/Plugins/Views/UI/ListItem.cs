#region Using Statements
using MarkLight.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkLight.Views.UI
{
    /// <summary>
    /// List object view.
    /// </summary>
    /// <d>Displays a selectable list object. Has the states: Default, Disabled, Highlighted, Pressed and Selected.</d>
    [HideInPresenter]
    public class ListItem : UIView
    {
        #region Fields

        #region ItemLabel

        /// <summary>
        /// List object text.
        /// </summary>
        /// <d>The text of the list object label.</d>
        [MapTo("ItemLabel.Text", "TextChanged")]
        public _string Text;

        /// <summary>
        /// List object text font.
        /// </summary>
        /// <d>The font of the list object label.</d>
        [MapTo("ItemLabel.Font")]
        public _Font Font;

        /// <summary>
        /// List object text font size.
        /// </summary>
        /// <d>The font size of the list object label.</d>
        [MapTo("ItemLabel.FontSize")]
        public _int FontSize;

        /// <summary>
        /// List object text line spacing.
        /// </summary>
        /// <d>The line spacing of the list object label.</d>
        [MapTo("ItemLabel.LineSpacing")]
        public _int LineSpacing;

        /// <summary>
        /// Supports rich text.
        /// </summary>
        /// <d>Boolean indicating if the list object label supports rich text.</d>
        [MapTo("ItemLabel.SupportRichText")]
        public _bool SupportRichText;

        /// <summary>
        /// List object text font color.
        /// </summary>
        /// <d>The font color of the list object label.</d>
        [MapTo("ItemLabel.FontColor")]
        public _Color FontColor;

        /// <summary>
        /// List object text font style.
        /// </summary>
        /// <d>The font style of the list object label.</d>
        [MapTo("ItemLabel.FontStyle")]
        public _FontStyle FontStyle;

        /// <summary>
        /// List object text margin.
        /// </summary>
        /// <d>The margin of the list object label. Can be used to adjust the text positioning.</d>
        [MapTo("ItemLabel.Margin")]
        public _ElementMargin TextMargin;

        /// <summary>
        /// List object text alignment.
        /// </summary>
        /// <d>The alignment of the text inside the list object label. Can be used with TextMargin and TextOffset to get desired positioning of the text.</d>
        [MapTo("ItemLabel.TextAlignment")]
        public _ElementAlignment TextAlignment;

        /// <summary>
        /// List object text offset.
        /// </summary>
        /// <d>The offset of the list object label. Can be used with TextMargin and TextAlignment to get desired positioning of the text.</d>
        [MapTo("ItemLabel.Offset")]
        public _ElementMargin TextOffset;

        /// <summary>
        /// List object text shadow color.
        /// </summary>
        /// <d>The shadow color of the list object label.</d>
        [MapTo("ItemLabel.ShadowColor")]
        public _Color ShadowColor;

        /// <summary>
        /// List object text shadow distance.
        /// </summary>
        /// <d>The distance of the list object label shadow.</d>
        [MapTo("ItemLabel.ShadowDistance")]
        public _Vector2 ShadowDistance;

        /// <summary>
        /// List object text outline color.
        /// </summary>
        /// <d>The outline color of the list object label.</d>
        [MapTo("ItemLabel.OutlineColor")]
        public _Color OutlineColor;

        /// <summary>
        /// List object text outline distance.
        /// </summary>
        /// <d>The distance of the list object label outline.</d>
        [MapTo("ItemLabel.OutlineDistance")]
        public _Vector2 OutlineDistance;

        /// <summary>
        /// Adjusts the list object to the text.
        /// </summary>
        /// <d>An enum indiciating how the list object should adjust its size to the label text.</d>
        [MapTo("ItemLabel.AdjustToText")]
        public _AdjustToText AdjustToText;

        /// <summary>
        /// The list object label.
        /// </summary>
        /// <d>The list object label displays text next to the list object.</d>
        public Label ItemLabel;

        #endregion

        /// <summary>
        /// Indicates if the object is disabled.
        /// </summary>
        /// <d>If set to true the object enters the "Disabled" state and can't be interacted with.</d>
        [ChangeHandler("IsDisabledChanged")]
        public _bool IsDisabled;

        /// <summary>
        /// Indicates if this object is an alternate object.
        /// </summary>
        /// <d>Boolean indicating if the tiem is an alternate object which uses the "Alternate" state instead of the "Default" state.</d>
        [ChangeHandler("IsAlternateChanged", TriggerImmediately = true)]
        public _bool IsAlternate;

        /// <summary>
        /// List object text padding.
        /// </summary>
        /// <d>Padding added to list object text when AdjustToText is set.</d>
        [ChangeHandler("TextChanged")]
        public _ElementMargin TextPadding;

        /// <summary>
        /// List object length.
        /// </summary>
        /// <d>Specifies the list object length. Used as the default object width when Width isn't set.</d>
        [ChangeHandler("LayoutsChanged")]
        public _ElementSize Length;

        /// <summary>
        /// List object breadth.
        /// </summary>
        /// <d>Specifies the list object breadth. Used as the default object height when Height isn't set.</d>
        [ChangeHandler("LayoutsChanged")]
        public _ElementSize Breadth;

        /// <summary>
        /// List object pool size.
        /// </summary>
        /// <d>Indicates how many list objects should be pooled. Pooled objects are already created and ready to be used rather than being created and destroyed on demand. Can be used to increase the performance of dynamic lists.</d>
        public _int PoolSize;

        /// <summary>
        /// Max list object pool size.
        /// </summary>
        /// <d>Indicates maximum number of list objects that should be pooled. If not set it uses initial PoolSize is used as max. Pooled objects are already created and ready to be used rather than being created and destroyed on demand. Can be used to increase the performance of dynamic lists.</d>
        public _int MaxPoolSize;

        /// <summary>
        /// Template used to create view.
        /// </summary>
        /// <d>Reference to the template used to create the view. Used to identify the list object type.</d>
        public View Template;

        [NotSetFromXuml]
        [ChangeHandler("IsSelectedChanged", TriggerImmediately = true)]
        public _bool IsSelected;

        [NotSetFromXuml]
        public _bool IsPressed;

        [NotSetFromXuml]
        public _bool IsMouseOver;

        /// <summary>
        /// Parent list.
        /// </summary>
        /// <d>The list that created this list object.</d>
        public List ParentList;

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            Breadth.DirectValue = new ElementSize(40);
            Length.DirectValue = new ElementSize(120);
            TextPadding.DirectValue = new ElementMargin();

            // list object label
            if (ItemLabel != null)
            {
                ItemLabel.TextAlignment.DirectValue = ElementAlignment.Center;
                ItemLabel.Width.DirectValue = ElementSize.FromPercents(1);
                ItemLabel.Height.DirectValue = ElementSize.FromPercents(1);
            }
        }

        /// <summary>
        /// Initializes the list object.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (ParentList == null)
            {
                ParentList = this.FindParent<List>();
            }
        }

        /// <summary>
        /// Called when the layout of the view has been changed. 
        /// </summary>
        public override void LayoutChanged()
        {
            // adjust width and height to ParentList
            if (ParentList == null || ParentList.Orientation == ElementOrientation.Horizontal)
            {
                Width.DirectValue = Width.IsSet && Width.Value.Unit != ElementSizeUnit.Percents ? Width.Value : new ElementSize(Length.Value);

                if (!Height.IsSet)
                {
                    Height.DirectValue = Breadth.IsSet ? new ElementSize(Breadth.Value) : ElementSize.FromPercents(1);
                }
            }
            else
            {
                // if neither width nor length is set, use 100% width                
                if (!Width.IsSet)
                {
                    Width.DirectValue = Length.IsSet ? new ElementSize(Length.Value) : ElementSize.FromPercents(1);
                }

                Height.DirectValue = Height.IsSet && Height.Value.Unit != ElementSizeUnit.Percents ? Height.Value : new ElementSize(Breadth.Value);
            }

            base.LayoutChanged();
        }

        /// <summary>
        /// Called when the object text has been changed.
        /// </summary>
        public virtual void TextChanged()
        {
            if (ItemLabel == null || ItemLabel.AdjustToText.Value == MarkLight.AdjustToText.None)
                return;
            
            // adjust object size to text
            if (ItemLabel.AdjustToText.Value == MarkLight.AdjustToText.Width)
            {
                Width.Value = new ElementSize(ItemLabel.PreferredWidth + TextPadding.Value.Left.Pixels + TextPadding.Value.Right.Pixels
                    + ItemLabel.Margin.Value.Left.Pixels + ItemLabel.Margin.Value.Right.Pixels);
            }
            else if (ItemLabel.AdjustToText.Value == MarkLight.AdjustToText.Height)
            {
                Height.Value = new ElementSize(ItemLabel.PreferredHeight + TextPadding.Value.Top.Pixels + TextPadding.Value.Bottom.Pixels
                    + ItemLabel.Margin.Value.Top.Pixels + ItemLabel.Margin.Value.Bottom.Pixels);
            }
            else if (ItemLabel.AdjustToText.Value == MarkLight.AdjustToText.WidthAndHeight)
            {
                Width.Value = new ElementSize(ItemLabel.PreferredWidth + TextPadding.Value.Left.Pixels + TextPadding.Value.Right.Pixels
                    + ItemLabel.Margin.Value.Left.Pixels + ItemLabel.Margin.Value.Right.Pixels);
                Height.Value = new ElementSize(ItemLabel.PreferredHeight + TextPadding.Value.Top.Pixels + TextPadding.Value.Bottom.Pixels
                    + ItemLabel.Margin.Value.Top.Pixels + ItemLabel.Margin.Value.Bottom.Pixels);
            }

            LayoutsChanged();
        }

        /// <summary>
        /// Called when mouse is clicked.
        /// </summary>
        public void ListItemMouseClick()
        {
            if (ParentList == null || State == "Disabled")
                return;

            if (!ParentList.SelectOnMouseUp.Value)
                return;

            ParentList.SelectItem(this, true);
        }

        /// <summary>
        /// Called when mouse enters.
        /// </summary>
        public void ListItemMouseEnter()
        {
            if (State == "Disabled")
                return;

            IsMouseOver.DirectValue = true;
            if (IsSelected)
                return;

            if (IsPressed)
            {
                SetState("Pressed");
            }
            else
            {
                SetState("Highlighted");
            }
        }

        /// <summary>
        /// Called when mouse exits.
        /// </summary>
        public void ListItemMouseExit()
        {
            if (State == "Disabled")
                return;

            IsMouseOver.DirectValue = false;
            if (IsSelected)
                return;

            SetState(DefaultItemStyle);
        }

        /// <summary>
        /// Called when mouse down.
        /// </summary>
        public void ListItemMouseDown()
        {
            if (ParentList == null || State == "Disabled")
                return;

            if (!ParentList.SelectOnMouseUp.Value)
            {
                ParentList.SelectItem(this, true);
            }
            else
            {
                IsPressed.DirectValue = true;
                if (IsSelected)
                    return;

                SetState("Pressed");
            }
        }

        /// <summary>
        /// Called when mouse up.
        /// </summary>
        public void ListItemMouseUp()
        {
            if (State == "Disabled")
                return;

            IsPressed.DirectValue = false;
            if (IsSelected)
                return;

            if (IsMouseOver)
            {
                SetState("Highlighted");
            }
            else
            {
                SetState(DefaultItemStyle);
            }
        }

        /// <summary>
        /// Called when the IsSelected field changes.
        /// </summary>
        public virtual void IsSelectedChanged()
        {
            if (State == "Disabled")
                return;

            if (IsSelected)
            {
                SetState("Selected");
            }
            else
            {
                SetState(DefaultItemStyle);
            }
        }

        /// <summary>
        /// Called when IsDisabled field changes.
        /// </summary>
        public virtual void IsDisabledChanged()
        {
            if (IsDisabled)
            {
                SetState("Disabled");

                // disable list object actions
                Click.IsDisabled = true;
                MouseEnter.IsDisabled = true;
                MouseExit.IsDisabled = true;
                MouseDown.IsDisabled = true;
                MouseUp.IsDisabled = true;
            }
            else
            {
                SetState(IsSelected ? "Selected" : DefaultItemStyle);

                // enable list object actions
                Click.IsDisabled = false;
                MouseEnter.IsDisabled = false;
                MouseExit.IsDisabled = false;
                MouseDown.IsDisabled = false;
                MouseUp.IsDisabled = false;
            }
        }

        /// <summary>
        /// Called when IsAlternate changed.
        /// </summary>
        public virtual void IsAlternateChanged()
        {
            if (IsSelected)
                return;

            SetState(DefaultItemStyle);
        }

        /// <summary>
        /// Sets the state of the view.
        /// </summary>
        public override void SetState(string state)
        {
            base.SetState(state);
            if (ItemLabel != null)
            {
                ItemLabel.SetState(state);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns default object style.
        /// </summary>
        public string DefaultItemStyle
        {
            get
            {
                return IsAlternate ? "Alternate" : "Default";
            }
        }

        #endregion
    }
}
