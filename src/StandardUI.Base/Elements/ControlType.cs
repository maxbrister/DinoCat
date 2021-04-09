using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public enum ControlType
    {
        Button,
        Calendar,
        CheckBox,
        ComboBox,
        Edit,
        Hyperlink,
        Image,
        ListItem,
        List,
        Menu,
        MenuBar,
        MenuItem,
        ProgressBar,
        RadioButton,
        ScrollBar,
        Slider,
        Spinner,
        StatusBar,
        Tab,
        TabItem,
        Text,
        ToolBar,
        ToolTip,
        Tree,
        TreeItem,
        Custom,
        Group,
        Thumb,
        DataGrid,
        DataItem,
        Document,
        SplitButton,
        Window,
        Pane,
        Header,
        HeaderItem,
        Table,
        TitleBar,
        Separator,

        /// <summary>
        /// Indicates the InputGroup should not be accessible
        /// </summary>
        /// <remarks>
        /// WARNING: InputGroups should normally not be marked as None. This will make it impossible
        /// for disabled users to provide input.
        /// </remarks>
        None = 9999
    }
}
