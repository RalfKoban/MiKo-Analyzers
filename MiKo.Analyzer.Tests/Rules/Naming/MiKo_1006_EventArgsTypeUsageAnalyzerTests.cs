using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1006_EventArgsTypeUsageAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WindowsPredefinedEventHandlers =
                                                                          {
                                                                              "System.Windows.Annotations.AnnotationAuthorChangedEventHandler",
                                                                              "System.Windows.Annotations.AnnotationResourceChangedEventHandler",
                                                                              "System.Windows.Annotations.Storage.StoreContentChangedEventHandler",
                                                                              "System.Windows.Controls.CleanUpVirtualizedItemEventHandler",
                                                                              "System.Windows.Controls.ContextMenuEventHandler",
                                                                              "System.Windows.Controls.DataGridSortingEventHandler",
                                                                              "System.Windows.Controls.InitializingNewItemEventHandler",
                                                                              "System.Windows.Controls.InkCanvasGestureEventHandler",
                                                                              "System.Windows.Controls.InkCanvasSelectionChangingEventHandler",
                                                                              "System.Windows.Controls.InkCanvasSelectionEditingEventHandler",
                                                                              "System.Windows.Controls.InkCanvasStrokeCollectedEventHandler",
                                                                              "System.Windows.Controls.InkCanvasStrokeErasingEventHandler",
                                                                              "System.Windows.Controls.InkCanvasStrokesReplacedEventHandler",
                                                                              "System.Windows.Controls.Primitives.DragCompletedEventHandler",
                                                                              "System.Windows.Controls.Primitives.DragDeltaEventHandler",
                                                                              "System.Windows.Controls.Primitives.DragStartedEventHandler",
                                                                              "System.Windows.Controls.Primitives.ItemsChangedEventHandler",
                                                                              "System.Windows.Controls.Primitives.ScrollEventHandler",
                                                                              "System.Windows.Controls.ScrollChangedEventHandler",
                                                                              "System.Windows.Controls.SelectedCellsChangedEventHandler",
                                                                              "System.Windows.Controls.SelectionChangedEventHandler",
                                                                              "System.Windows.Controls.TextChangedEventHandler",
                                                                              "System.Windows.Controls.ToolTipEventHandler",
                                                                              "System.Windows.Data.FilterEventHandler",
                                                                              "System.Windows.Documents.GetPageRootCompletedEventHandler",
                                                                              "System.Windows.Documents.Serialization.WritingCancelledEventHandler",
                                                                              "System.Windows.Documents.Serialization.WritingCompletedEventHandler",
                                                                              "System.Windows.Documents.Serialization.WritingPrintTicketRequiredEventHandler",
                                                                              "System.Windows.Documents.Serialization.WritingProgressChangedEventHandler",
                                                                              "System.Windows.ExitEventHandler",
                                                                              "System.Windows.Forms.BindingCompleteEventHandler",
                                                                              "System.Windows.Forms.BindingManagerDataErrorEventHandler",
                                                                              "System.Windows.Forms.CacheVirtualItemsEventHandler",
                                                                              "System.Windows.Forms.ColumnClickEventHandler",
                                                                              "System.Windows.Forms.ColumnReorderedEventHandler",
                                                                              "System.Windows.Forms.ColumnWidthChangedEventHandler",
                                                                              "System.Windows.Forms.ColumnWidthChangingEventHandler",
                                                                              "System.Windows.Forms.ContentsResizedEventHandler",
                                                                              "System.Windows.Forms.ControlEventHandler",
                                                                              "System.Windows.Forms.ConvertEventHandler",
                                                                              "System.Windows.Forms.DataGridViewAutoSizeColumnModeEventHandler",
                                                                              "System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventHandler",
                                                                              "System.Windows.Forms.DataGridViewAutoSizeModeEventHandler",
                                                                              "System.Windows.Forms.DataGridViewBindingCompleteEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellCancelEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellErrorTextNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellFormattingEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellMouseEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellPaintingEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellParsingEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellStateChangedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellStyleContentChangedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellValidatingEventHandler",
                                                                              "System.Windows.Forms.DataGridViewCellValueEventHandler",
                                                                              "System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventHandler",
                                                                              "System.Windows.Forms.DataGridViewColumnEventHandler",
                                                                              "System.Windows.Forms.DataGridViewColumnStateChangedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewDataErrorEventHandler",
                                                                              "System.Windows.Forms.DataGridViewEditingControlShowingEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowCancelEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowDividerDoubleClickEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowErrorTextNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowHeightInfoNeededEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowHeightInfoPushedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowPostPaintEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowPrePaintEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowsAddedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowsRemovedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewRowStateChangedEventHandler",
                                                                              "System.Windows.Forms.DataGridViewSortCompareEventHandler",
                                                                              "System.Windows.Forms.DateBoldEventHandler",
                                                                              "System.Windows.Forms.DateRangeEventHandler",
                                                                              "System.Windows.Forms.DpiChangedEventHandler",
                                                                              "System.Windows.Forms.DragEventHandler",
                                                                              "System.Windows.Forms.DrawItemEventHandler",
                                                                              "System.Windows.Forms.DrawListViewColumnHeaderEventHandler",
                                                                              "System.Windows.Forms.DrawListViewItemEventHandler",
                                                                              "System.Windows.Forms.DrawListViewSubItemEventHandler",
                                                                              "System.Windows.Forms.DrawToolTipEventHandler",
                                                                              "System.Windows.Forms.DrawTreeNodeEventHandler",
                                                                              "System.Windows.Forms.FormClosedEventHandler",
                                                                              "System.Windows.Forms.FormClosingEventHandler",
                                                                              "System.Windows.Forms.GiveFeedbackEventHandler",
                                                                              "System.Windows.Forms.HelpEventHandler",
                                                                              "System.Windows.Forms.HtmlElementErrorEventHandler",
                                                                              "System.Windows.Forms.HtmlElementEventHandler",
                                                                              "System.Windows.Forms.InputLanguageChangedEventHandler",
                                                                              "System.Windows.Forms.InputLanguageChangingEventHandler",
                                                                              "System.Windows.Forms.InvalidateEventHandler",
                                                                              "System.Windows.Forms.ItemChangedEventHandler",
                                                                              "System.Windows.Forms.ItemCheckedEventHandler",
                                                                              "System.Windows.Forms.ItemCheckEventHandler",
                                                                              "System.Windows.Forms.ItemDragEventHandler",
                                                                              "System.Windows.Forms.KeyEventHandler",
                                                                              "System.Windows.Forms.KeyPressEventHandler",
                                                                              "System.Windows.Forms.LabelEditEventHandler",
                                                                              "System.Windows.Forms.LayoutEventHandler",
                                                                              "System.Windows.Forms.LinkClickedEventHandler",
                                                                              "System.Windows.Forms.LinkLabelLinkClickedEventHandler",
                                                                              "System.Windows.Forms.ListControlConvertEventHandler",
                                                                              "System.Windows.Forms.ListViewItemMouseHoverEventHandler",
                                                                              "System.Windows.Forms.ListViewItemSelectionChangedEventHandler",
                                                                              "System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler",
                                                                              "System.Windows.Forms.MaskInputRejectedEventHandler",
                                                                              "System.Windows.Forms.MeasureItemEventHandler",
                                                                              "System.Windows.Forms.MouseEventHandler",
                                                                              "System.Windows.Forms.NavigateEventHandler",
                                                                              "System.Windows.Forms.NodeLabelEditEventHandler",
                                                                              "System.Windows.Forms.PaintEventHandler",
                                                                              "System.Windows.Forms.PopupEventHandler",
                                                                              "System.Windows.Forms.PreviewKeyDownEventHandler",
                                                                              "System.Windows.Forms.PropertyTabChangedEventHandler",
                                                                              "System.Windows.Forms.PropertyValueChangedEventHandler",
                                                                              "System.Windows.Forms.QueryAccessibilityHelpEventHandler",
                                                                              "System.Windows.Forms.QueryContinueDragEventHandler",
                                                                              "System.Windows.Forms.QuestionEventHandler",
                                                                              "System.Windows.Forms.RetrieveVirtualItemEventHandler",
                                                                              "System.Windows.Forms.ScrollEventHandler",
                                                                              "System.Windows.Forms.SearchForVirtualItemEventHandler",
                                                                              "System.Windows.Forms.SelectedGridItemChangedEventHandler",
                                                                              "System.Windows.Forms.SplitterCancelEventHandler",
                                                                              "System.Windows.Forms.SplitterEventHandler",
                                                                              "System.Windows.Forms.StatusBarDrawItemEventHandler",
                                                                              "System.Windows.Forms.StatusBarPanelClickEventHandler",
                                                                              "System.Windows.Forms.TabControlCancelEventHandler",
                                                                              "System.Windows.Forms.TabControlEventHandler",
                                                                              "System.Windows.Forms.TableLayoutCellPaintEventHandler",
                                                                              "System.Windows.Forms.ToolBarButtonClickEventHandler",
                                                                              "System.Windows.Forms.ToolStripArrowRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripContentPanelRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripDropDownClosedEventHandler",
                                                                              "System.Windows.Forms.ToolStripDropDownClosingEventHandler",
                                                                              "System.Windows.Forms.ToolStripGripRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripItemClickedEventHandler",
                                                                              "System.Windows.Forms.ToolStripItemEventHandler",
                                                                              "System.Windows.Forms.ToolStripItemImageRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripItemRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripItemTextRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripPanelRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripRenderEventHandler",
                                                                              "System.Windows.Forms.ToolStripSeparatorRenderEventHandler",
                                                                              "System.Windows.Forms.TreeNodeMouseClickEventHandler",
                                                                              "System.Windows.Forms.TreeNodeMouseHoverEventHandler",
                                                                              "System.Windows.Forms.TreeViewCancelEventHandler",
                                                                              "System.Windows.Forms.TreeViewEventHandler",
                                                                              "System.Windows.Forms.TypeValidationEventHandler",
                                                                              "System.Windows.Forms.UICuesEventHandler",
                                                                              "System.Windows.Forms.UpDownEventHandler",
                                                                              "System.Windows.Forms.WebBrowserDocumentCompletedEventHandler",
                                                                              "System.Windows.Forms.WebBrowserNavigatedEventHandler",
                                                                              "System.Windows.Forms.WebBrowserNavigatingEventHandler",
                                                                              "System.Windows.Forms.WebBrowserProgressChangedEventHandler",
                                                                              "System.Windows.Markup.Localizer.BamlLocalizerErrorNotifyEventHandler",
                                                                              "System.Windows.Navigation.FragmentNavigationEventHandler",
                                                                              "System.Windows.Navigation.LoadCompletedEventHandler",
                                                                              "System.Windows.Navigation.NavigatedEventHandler",
                                                                              "System.Windows.Navigation.NavigatingCancelEventHandler",
                                                                              "System.Windows.Navigation.NavigationFailedEventHandler",
                                                                              "System.Windows.Navigation.NavigationProgressEventHandler",
                                                                              "System.Windows.Navigation.NavigationStoppedEventHandler",
                                                                              "System.Windows.Navigation.RequestNavigateEventHandler",
                                                                              "System.Windows.RequestBringIntoViewEventHandler",
                                                                              "System.Windows.SessionEndingCancelEventHandler",
                                                                              "System.Windows.SizeChangedEventHandler",
                                                                              "System.Windows.StartupEventHandler",
                                                                          };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_event_with_generic_EventHandler_using_correct_EventArgs() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler<MyEventArgs> My;
}
");

        [Test]
        public void No_issue_is_reported_for_predefined_CanExecuteChanged_EventHandler() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;
using System.Collections.Specialized;

public class TestMe
{
    public event EventHandler CanExecuteChanged;
}
");

        [Test]
        public void An_issue_is_reported_for_event_with_generic_EventHandler_using_incorrect_EventArgs() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler<MyEventArgs> MyOwn;
}
");

        [Test]
        public void An_issue_is_reported_for_non_generic_EventHandler() => An_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs { }

public class TestMe
{
    public event EventHandler My;
}
");

        [Test, Ignore("Currently cannot be tested")]
        public void No_issue_is_reported_for_interface_implementation() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

namespace Bla
{
    public abstract class TestMe : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_predefined_WindowsForms_orWindowsControls_event_([ValueSource(nameof(WindowsPredefinedEventHandlers))] string eventHandlerType) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event " + eventHandlerType + @" MyEvent;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1006_EventArgsTypeUsageAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1006_EventArgsTypeUsageAnalyzer();
    }
}