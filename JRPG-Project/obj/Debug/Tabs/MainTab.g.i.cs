﻿#pragma checksum "..\..\..\Tabs\MainTab.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B7012774BE72E948ED4751A110B07D3FE0D9F60F6545E6089051FCF7AA6A8E84"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using JRPG_Project.Tabs;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace JRPG_Project.Tabs {
    
    
    /// <summary>
    /// MainTab
    /// </summary>
    public partial class MainTab : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\Tabs\MainTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnDispatch;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Tabs\MainTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnInventory;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Tabs\MainTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnTeam;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Tabs\MainTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSettings;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Tabs\MainTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSave;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/JRPG-Project;component/tabs/maintab.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Tabs\MainTab.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.BtnDispatch = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\Tabs\MainTab.xaml"
            this.BtnDispatch.Click += new System.Windows.RoutedEventHandler(this.OpenTab);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BtnInventory = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\Tabs\MainTab.xaml"
            this.BtnInventory.Click += new System.Windows.RoutedEventHandler(this.OpenTab);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BtnTeam = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.BtnSettings = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.BtnSave = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

