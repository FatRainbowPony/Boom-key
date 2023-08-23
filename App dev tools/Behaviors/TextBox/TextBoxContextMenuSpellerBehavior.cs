using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using AppDevTools.Controls.MenuItems;
using sysControls = System.Windows.Controls;

namespace AppDevTools.Behaviors.TextBox
{
    public class TextBoxContextMenuSpellerBehavior : Behavior<sysControls.TextBox>
    {
        #region Properties

        #region Public

        #region Property for default style context menu
        public static readonly DependencyProperty DefContextMenuStyleProperty =
            DependencyProperty.Register(nameof(DefContextMenuStyle), typeof(Style), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public Style DefContextMenuStyle
        {
            get { return (Style)GetValue(DefContextMenuStyleProperty); }
            set { SetValue(DefContextMenuStyleProperty, value); }
        }
        #endregion Property for default style context menu

        #region Property for default template menu item
        public static readonly DependencyProperty DefMenuItemTemplateProperty =
            DependencyProperty.Register(nameof(DefMenuItemTemplate), typeof(ControlTemplate), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public ControlTemplate DefMenuItemTemplate
        {
            get { return (ControlTemplate)GetValue(DefMenuItemTemplateProperty); }
            set { SetValue(DefMenuItemTemplateProperty, value); }
        }
        #endregion Property for default template menu item

        #region Property for default template style separator
        public static readonly DependencyProperty DefSeparatorStyleProperty =
            DependencyProperty.Register(nameof(DefSeparatorStyle), typeof(Style), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public Style DefSeparatorStyle
        {
            get { return (Style)GetValue(DefSeparatorStyleProperty); }
            set { SetValue(DefSeparatorStyleProperty, value); }
        }
        #endregion Property for default template style separator

        #region Property for default color icon context menu
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register(nameof(IconColor), typeof(SolidColorBrush), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public SolidColorBrush IconColor
        {
            get { return (SolidColorBrush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }
        #endregion Property for default color icon context menu

        #region Property for icon cut command
        public static readonly DependencyProperty IconCutProperty =
            DependencyProperty.Register(nameof(IconCut), typeof(GeometryGroup), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public GeometryGroup IconCut
        {
            get { return (GeometryGroup)GetValue(IconCutProperty); }
            set { SetValue(IconCutProperty, value); }
        }
        #endregion Property for icon cut command

        #region Property for icon copy command
        public static readonly DependencyProperty IconCopyProperty =
            DependencyProperty.Register(nameof(IconCopy), typeof(GeometryGroup), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public GeometryGroup IconCopy
        {
            get { return (GeometryGroup)GetValue(IconCopyProperty); }
            set { SetValue(IconCopyProperty, value); }
        }
        #endregion Property for icon copy command

        #region Property for icon paste command
        public static readonly DependencyProperty IconPasteProperty =
            DependencyProperty.Register(nameof(IconPaste), typeof(GeometryGroup), typeof(TextBoxContextMenuSpellerBehavior), new PropertyMetadata(null));

        public GeometryGroup IconPaste
        {
            get { return (GeometryGroup)GetValue(IconPasteProperty); }
            set { SetValue(IconPasteProperty, value); }
        }
        #endregion Property for icon paste command

        #endregion Public

        #endregion Properties

        #region Methods

        #region Private
        private void AssociatedObjectContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            sysControls.TextBox textBox = (sysControls.TextBox)sender;
            if (textBox != null)
            {
                int caretIndex, contextMenuIndex;
                SpellingError spellingError;

                textBox.ContextMenu = GetContextMenu();
                caretIndex = textBox.CaretIndex;
                contextMenuIndex = 0;
                spellingError = textBox.GetSpellingError(caretIndex);
                if (spellingError != null)
                {
                    foreach (string str in spellingError.Suggestions)
                    {
                        VectorIconMenuItem menuItem = new()
                        {
                            Template = DefMenuItemTemplate,
                            VectorIconVisibility = Visibility.Visible,
                            VectorIconSize = 13,
                            VectorIconStretch = Stretch.None,
                            Header = str,
                            FontWeight = FontWeights.Bold,
                            Command = EditingCommands.CorrectSpellingError,
                            CommandParameter = str,
                            CommandTarget = textBox
                        };
                        textBox.ContextMenu.Items.Insert(contextMenuIndex, menuItem);
                        contextMenuIndex++;
                    }

                    Separator separatorMenuItem1 = new() { Style = DefSeparatorStyle };
                    textBox.ContextMenu.Items.Insert(contextMenuIndex, separatorMenuItem1);
                    contextMenuIndex++;
                    VectorIconMenuItem ignoreAllMenuItem = new()
                    {
                        Template = DefMenuItemTemplate,
                        VectorIconVisibility = Visibility.Visible,
                        VectorIconSize = 13,
                        VectorIconStretch = Stretch.None,
                        Header = "Пропустить все",
                        Command = EditingCommands.IgnoreSpellingError,
                        CommandTarget = textBox
                    };
                    textBox.ContextMenu.Items.Insert(contextMenuIndex, ignoreAllMenuItem);
                    contextMenuIndex++;
                    Separator separatorMenuItem2 = new() { Style = DefSeparatorStyle };
                    textBox.ContextMenu.Items.Insert(contextMenuIndex, separatorMenuItem2);
                }
            }

            ContextMenu GetContextMenu()
            {
                ContextMenu contextMenu = new() { Style = DefContextMenuStyle };

                VectorIconMenuItem menuItem1 = new()
                {
                    Template = DefMenuItemTemplate,
                    VectorIcon = IconCut,
                    VectorIconFill = IconColor,
                    VectorIconSize = 13,
                    VectorIconStretch = Stretch.UniformToFill,
                    Command = ApplicationCommands.Cut,
                };

                VectorIconMenuItem menuItem2 = new()
                {
                    Template = DefMenuItemTemplate,
                    VectorIcon = IconCopy,
                    VectorIconFill = IconColor,
                    VectorIconSize = 13,
                    VectorIconStretch = Stretch.UniformToFill,
                    Command = ApplicationCommands.Copy
                };

                VectorIconMenuItem menuItem3 = new()
                {
                    Template = DefMenuItemTemplate,
                    VectorIcon = IconPaste,
                    VectorIconFill = IconColor,
                    VectorIconSize = 13,
                    VectorIconStretch = Stretch.UniformToFill,
                    Command = ApplicationCommands.Paste
                };

                contextMenu.Items.Add(menuItem1);
                contextMenu.Items.Add(menuItem2);
                contextMenu.Items.Add(menuItem3);

                return contextMenu;
            }
        }
        #endregion Private

        #region Protected
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ContextMenuOpening += AssociatedObjectContextMenuOpening;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ContextMenuOpening -= AssociatedObjectContextMenuOpening;
            base.OnDetaching();
        }
        #endregion Protected

        #endregion Methods
    }
}