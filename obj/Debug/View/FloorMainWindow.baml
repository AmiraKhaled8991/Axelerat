   M S B A M L   `   `   `  ����D  @Axelerate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Axelerate.ViewAxelerate.View  5   	   $    Axelerate.View.MainWindowFloor   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Axelerate.View     5   	     � Title$  MainWindowFloor��5   	   $	��250��6!   $	��200��6.   .��5	      � .+�5
   
   �� $��50��6   $	��100��6   $��Bottom=�6)   ��6D   �� *CreateCommand+.��6f   create6n   �� 5   
   $��15(�6   
�������$��Create Floor��64   $��2��6J   $��4��6^   .��5      �� $��2��6   $��2��6-    �� Orientation$ Vertical=�6:   .+�5      �� $��0��6   $
��Left=�6,   $	��120��6G   $
��0 10q�6S   Select Floor Type :6w   �� 5      -   5        $��selectedFloorTypeCB��5   �   $��Right=�5      $��0 -00q�68   $��2q�6G   $��25��6S   $	��170��6_   ��6k   �� *revitFloorList+ �� SelectedItem 5   #   �� *selectedFloorType+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   5      5      5      5      