   M S B A M L   `   `   `  ����J  FRunningReporter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:RunningReporterRunningReporter  5           RunningReporter.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      9runningReporterclr-namespace:RunningReporter     5        � Title$  Running Reporter��5       � 	Resources 5
         mCommandButtonStyle �� 
TargetType �� BasedOn �� Property �� Value��%�   &	        7���� 5   
   " ��6,   8   .�5      ��  � �� 6(   �� *	Command+�� 5       � �� 6(   �� *DisplayText+5      5       � DataContext 5      '  !RunningReporter.ReporterViewModel  5   
   .��5      ]� .+�5   
   �� $	��Top=�6   $��DropBox Path��6(   .��5      �� ��6   �� *BaseDirectoryPath+ �� UpdateSourceTrigger$ PropertyChanged=�5      �� 5   
   $	��Top=�6   $��
Time Range��6(   .��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� #��[  6E    6   �� *PreviousWeekCommand+6E   �� 5      $
��From��6   $��Center=�6'   (  "System.Windows.Controls.DatePicker  5      	   SelectedDate	 6   �� *StartDateInclusive+�� 5      $��Through��6   $��Center=�6*     5      	 6   �� *EndDateInclusive+�� 5       #��[  6A    6   �� *NextWeekCommand+6A   5!      5"      �� 5$   
   $��Bottom=�6   #��[  6R    6)   �� *ClipboardCommand+6R   �� 5&   
   $��Results��6   .��5'      (  "System.Windows.Controls.WebBrowser  %  RunningReporter.BrowserBehavior
   Html
 6   �� *RenderedHtml+5(      5*      5,      