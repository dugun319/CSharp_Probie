   M S B A M L   `   `   `  ����D  @ExpenseIt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ExpenseIt	ExpenseIt  5          ExpenseIt.ExpenseItHome   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:ExpenseIt     5        N� 
Background
  ����� N� Title$ ExpenseIt - Home��5	      .��5      � $��
10,0,10,10q�6   ��5   
   �� 5       �� ImageSource$ watermark.png��6    � 	Resources 5   
      nameItemTemplate �� XPath  ExpenseDataSource � XPath��%  &	        &	 B     �� 5      .R�5      �� ��6   �� $ @Name��5      � 5      $ Expenses��5   !   .��5      �	�<Expenses xmlns="">
                        <Person Name="Mike" Department="Legal">
                            <Expense ExpenseType="Lunch" ExpenseAmount="50" />
                            <Expense ExpenseType="Transportation" ExpenseAmount="50" />
                        </Person>

                        <Person Name="Lisa" Department="Marketing">
                            <Expense ExpenseType="Document printing" ExpenseAmount="50" />
                            <Expense ExpenseType="Gift" ExpenseAmount="125" />
                        </Person>

                        <Person Name="John" Department="Engineering">
                            <Expense ExpenseType="Magazine subscription" ExpenseAmount="50" />
                            <Expense ExpenseType="New machine" ExpenseAmount="600" />
                            <Expense ExpenseType="Software" ExpenseAmount="500" />
                        </Person>

                        <Person Name="Mary" Department="Finance">
                            <Expense ExpenseType="Dinner" ExpenseAmount="100" />
                        </Person>
                    </Expenses>        5:      5;       � ColumnDefinitions 5=   
   �� 5>      $	��230 �6!   �� 5?       � RowDefinitions 5B   
   �� 5C      �� 5D      $
��Auto �6   �� 5E      �� 5F      $
��Auto �6   .+�5I   
   �� $��1��6     headerTextStyle#��[ 5J      .��69   View Expense Report5L      �� 5N   
   $��1��6   $��1��5O        listHeaderStyle#��[ 5P      ._�5Q      ��   listHeaderTextStyle#��[ 6   .��6A   Names5R      5S      �� 5T   
   -   5        $��peopleListBox��5T      $��1��5U      $��2��5V      #��[  5X      ��5W      ��  �� Source# [ $ Person��5Y      �� 5[   
   -   5        $��1��5[      $��3��5\        buttonStyle#��[ 5^      .��66   View5_      5b      5c      