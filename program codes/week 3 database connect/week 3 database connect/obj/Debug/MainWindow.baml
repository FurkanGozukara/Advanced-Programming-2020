   M S B A M L   `   `   `  ����R  Nweek 3 database connect, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:week_3_database_connectweek_3_database_connect  5   	   (    "week_3_database_connect.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:week_3_database_connect     5   	     � Title$  
MainWindow��5   	   $��525.42��6   $��808.8��6,   .��5	      � .+�5
   
   �� -   5        $��txtEmail��5
      $
��Left=�6"   $��23��6=   $��	22,20,0,0q�6I    �� TextWrapping$
 Wrap=�6\   $��
type email��6p   $	��Top=�6�   $	��120��6�   G� 5   
   -   5        $��pwUserPassword��5      $
��Left=�6,   $��
337,20,0,0q�6G   $	��Top=�6[   $��23��6s   $	��138��6   �� 5   
   $��type your password:��6   $
��Left=�6.   $��
202,20,0,0q�6I   $	��Top=�6]   �� 5   
   -   5        $��txtFirstName��5      $
��Left=�6'   $��23��6B   $��	22,61,0,0q�6N   $
 Wrap=�6a   $��type your first name��6u   $	��Top=�6�   $	��120��6�   �� 5   
   -   5         ��Name$ 
txtSurname��5      $
��Left=�6'   $��23��6B   $��
156,62,0,0q�6N   $
 Wrap=�6b   $��type your surname��6v   $	��Top=�6�   $	��120��6�   �� 5   
   -   5        $��cmbBoxUserRank��5      $
��Left=�6)   $��
300,62,0,0q�6D   $	��Top=�6X   $	��185��6p   �� 5   
   -   5         ��Name$ btnRegister��5      $��register��6&   $
��Left=�69   $��
512,62,0,0q�6T   $	��Top=�6h   $��75��6�   6�   &   System.Windows.Controls.DataGrid  5   
   -   5        $��dgUsers��5      $
��Left=�6"   $	��136��6=   $��
22,151,0,0q�6J   $	��Top=�6^   $	��692��6v   �� 5   
   -   5        $ 
btnRefresh��5      $��refresh��6%   $
��Left=�67   $��512,109,0,0q�6R   $	��Top=�6g   $	��137��6   $��37��6�   6�     5   
   -	   5          Name$ dgUsers2��5      $
��Left=�6%   $	��136��6@   $��
22,318,0,0q�6M   $	��Top=�6a   $	��692��6y   �� 5   
   -
   5        $ btnsave��5      $
��save��6"   $
��Left=�61   $��362,109,0,0q�6L   $	��Top=�6a   $	��113��6y   $��37��6�   6�   5      5      