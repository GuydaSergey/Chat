   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��10* �6   �� 5      $	��80* �6   �� 5      $	��10* �6   .+�5   
   � $��0��6   .+�5      �� -   5        $��btConn��5   '   $��Connect��6   $	��100��65   $��5q�6A   6L   �� 5      -   5        $��	btDisConn��5   +   $��
Disconnect��6   $	��100��6<   $��5q�6H   6S   5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $	��20* �6#   �� 5      $	��60* �6#   .+�5      �� -   5        $��ChatText��5   &   $��1��6   " �� VerticalScrollBarVisibility$
 Auto=�66   �� 5      -   5        $��ListView��5   '   $��0��6   6'   5      � 5   
   $��2��6    5      �� 5      $	��80* �6#   �� 5      $	��20* �6#   .+�5       �� -   5        $��SendText��5    &   $��0��6   $��20(�66   $
 Auto=�6D   �� 5!      -   5        $
��Send��5!   &   $��1��6   $
��Send��62   6B   5"      5#      5$      