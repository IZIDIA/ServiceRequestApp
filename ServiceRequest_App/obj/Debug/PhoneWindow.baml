   M S B A M L   `   `   `  ����M  IServiceRequest_App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:ServiceRequest_AppServiceRequest_App  5   	   $    ServiceRequest_App.PhoneWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:ServiceRequest_App     5   	   -   5          � Title$6  0Список контактных номеров��5   
   $	��560��6,   $	��790��69    � WindowStartupLocation$ CenterScreen=�5	   	   $	��560��6.   $	��420��6>    � 
ResizeMode$ 	CanResize=�5
   
    � WindowStyle$ SingleBorderWindow=�6!   $��Microsoft Sans Serif)�6B    � 	Resources 5         listTemplate �� CornerRadius �� Orientation �� Path � RowDefinitions	 �� 
IsReadOnly
 �� TextWrapping  listPanelTemplate��%{  &	        &	 ?    �� 5   
   .R�5      �� $ 8��6   $	��360��6&   $	��250��62   ���#191919._�5      �� $ 
Horizontal=�6   $��0,0,0,0q�66   .+�5      �� $��5,10,0,0q�6!   $	��100��63   $	��130��6?   .+�5      �� $��0,0,0,0q�6    $��80��61   $��80��6<   ��6H   �� $ 	ImagePath��� 5      $��0,5,0,0q�6$   $��Center=�65   $��14(�6R   
��� ���$
��Wrap=�5   ^   ��6,   �� $ 	PhoneType�6^   5      � 5      $	��Top=�6   $��	20,10,0,0q�64   $��Right=�6G   $	��225��5   $   $	��230��60    5      �� 5      $��45 �6,   6:   �� 5      $��65 �6,   6:   �� 5      6.   .+�5      �� $��0q�6"   
��� ���
������ $��Disabled=�6a   $
��Auto=�6�   	 . $
��Left=�5   /   $��Center=�6J   $

 Wrap=�6e   $��0��6y   $��25(�6�   ��5   ,   �� $ PhoneCallNumber��� 5      $��0q�6"   
�������
������ $��Disabled=�6c   $
��Auto=�6�   	 . $
��Left=�5    /   $��Center=�6J   $

 Wrap=�6e   $��1��6y   $��17(�6�   ��5!   ,   �� $ PhoneUserName��� 5"      $��0,5,0,0q�6"   $��0q�63   
�������
������ $��Disabled=�6s   $
��Auto=�6�   	 . $
��Left=�5#   0   $��Center=�6K   $

 Wrap=�6f   $��2��6z   $��16(�6�   ��5$   ,   �� $ PhoneDescription�5%      5&      5'      5(      �� 5)   
   .R�5*      � 5+      5,      .��50      � ���#313131.+�52   
   �� ���#212121$ 8��6'   $��5,5,5,5q�68   ._�53      �� -   5         ��Name$ 
phonesList��53   =   $��0q�6   
�������
������ #��[ 54      $��Disabled=�55      #��[  56      59      5;      5<      