   M S B A M L   `   `   `  ����D  @ExpenseIt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ExpenseIt	ExpenseIt  5      #    ExpenseIt.ExpenseItReportPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:ExpenseIt     5        N� 
Background
  ����� N� Title$ ExpenseIt - View Expense��5
      .��5      � ��5   
   �� 5       �� ImageSource$ watermark.png��6    � ColumnDefinitions 5   
   �� 5      $	��230 �6!   �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � 	Resources 5   
      typeItemTemplate �� XPath  amountItemTemplate��%�   &	        &	 I     �� 5      .R�5      �� ��6   �� $ @ExpenseType��5      �� 5       .R�5!      �� ��6   �� $ @ExpenseAmount��5"      5#      .+�5%   
   �� $��1��6     headerTextStyle#��[ 5&      .��69   Expense Report For:5(      � 5*   
   $��10q�6   $��1��5+      $��1��5,       5.      �� 5/      �� 50       52      �� 53      $
��Auto �6    �� 54      $
��Auto �6    �� 55      .+�58      �� $��0��6   $��2��59      $��0��5:       �� Orientation$ 
Horizontal=�5;      .+�5<      ��   
labelStyle#��[ 6   .��6=   Name :5>      �� 5?      #��[ 6   ��5@      �� $ @Name��5A      5B      �� 5D      $��0��6   $��2��5E      $��1��5F      $ 
Horizontal=�5G      .+�5H      �� #��[ 6   .��6<   Department:5J      �� 5K      #��[ 6   ��5L      �� $ @Department��5M      5N      � 5P      $��0��6   $��2��5Q      $��2��5R      $	��Top=�5S      $
��Left=�5T      .+�5W      &   System.Windows.Controls.DataGrid    columnHeaderStyle   ColumnHeaderStyle# [ 6   	   AutoGenerateColumns	 .  
   RowHeaderWidth$
 0Q�5Z      ��5X      �� $ Expense��   Columns 5\      4  .System.Windows.Controls.DataGridTemplateColumn  5]      ,  &System.Windows.Controls.DataGridColumn   Header$ ExpenseType��61      CellTemplate# [  5^   1     5_      $ Amount��61   # [ 5`   1   5b      5c      5d      5e      5f      