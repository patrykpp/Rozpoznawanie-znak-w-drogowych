   M S B A M L   `   `   `  ����b  ^Rozpoznawanie znaków - Praca dyplomowa, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Rozpoznawanie_znaków___Praca_dyplomowa'Rozpoznawanie_znaków___Praca_dyplomowa  5      3    -Rozpoznawanie_znaków___Praca_dyplomowa.Page3   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Glocal5clr-namespace:Rozpoznawanie_znaków___Praca_dyplomowa     5        N� Title$  Page2��5      $	��754��6   $	��476��6!   .��5	      � $��
0,-1,2.6,6q�6   ��5
   
   �� 5       �� Color# � ��6   .+�5   
   �� -   5         ��Name$ btn_back��5      $��Cofnij��6#   $
��Left=�64   $��
18,414,0,0q�6O   $	��Top=�6c   $	��142��6{   ���	#FF41B73A���	#FF41B73A$��43��6�    J� RenderTransformOrigin$ 0.838,0.486&�6�   �� 5   
   -   5        $ 	btn_play_��5      $
��Left=�6$   $��483,410,0,0q�6?   $	��Top=�6T   $��89��6l   ���	#FF3AA3B7$��48��6�   $ 0.838,0.486&�6�    � ToolTip$
 Play��6�   ��6�   Z� .��5      �� -   5         ��Name$ btn_play��5      $��Main_Images/btn_play.png��6&   $
��Fill=�6H   $ 1.705,0.824&�6W   $��38��6{   $��33��6�   $��2��6�   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ SignBox��5      $
��Left=�6$   $��	-3,18,0,0q�6?   $	��Top=�6R   $	��381��6j   $	��188��6w   $��14(�6�      
IsReadOnly . $   HorizontalScrollBarVisibility$ Disabled=�6�   ��6�   Z� 	   Columns	 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 SizeToCells 6)      
IsReadOnly .    CellTemplate 5      �� 5      .R�5      �� $	��150��6H   $	��150��6T   ��6$   �� *Rozpoznane znaki+6T   5      5      ��5      �� 5      # � ��6"   5      �� 5   
   -   5        $ btn_Stop��5      $
��Left=�6#   $��326,409,0,0q�6>   $	��Top=�6S   $��74��6k   ���	#FF3AA3B7$��48��6�   $ 0.838,0.486&�6�   ��6�   Z� .��5       �� -   5        $ btn_stop��5       $��Main_Images/btn_stop.png��6&   $
��Fill=�6H   $ 
1.464,0.87&�6W   $��43��6z   $��33��6�   $��2��6�   5!      �� 5"   
   -   5        $ 
btn_pause_��5"      $
��Left=�6%   $��404,410,0,0q�6@   $	��Top=�6U   $��74��6m   ���	#FF3AA3B7$��48��6�   $ 0.838,0.486&�6�   ��6�   Z� .��5#      �� -   5        $ 	btn_pause��5#      $��Main_Images/btn_pause.png��6'   $
��Fill=�6J   $ 
1.464,0.87&�6Y   $��43��6|   $��33��6�   $��2��6�   5$      r� 5%   
   -	   5         r�Name$ mediaControl��5%      $
��Left=�6-   $	��321��6H   $��
186,80,0,0q�6U   $	��Top=�6i   $	��565��6�   �� 5&   
   -
   5        $ img_control��5&      $
��Left=�6%   $��27��6@   $��174,409,0,0q�6L   $	��Top=�6a   $��42��6y   $��Main_Images/start.png��6�   �� 5'   
   -   5         ��Name$ label��5'      $1��+Liczba zidentyfikowanych obiektów z bazy :��6   $
��Left=�6T   $��
207,11,0,0q�6o   $	��Top=�6�   ���	#DD3ADF06$��18(�6�   �� 5(   
   -   5        $ label2��5(      $&�� Liczba znalezionych kandydatów:��6    $
��Left=�6J   $��
207,36,0,0q�6e   $	��Top=�6y   $��18(�6�   ���	#DDDDE51D6�   �� 5)   
   -   5        $ label_countSigns��5)      $��Label��6*   $
��Left=�6:   $��
560,15,0,0q�6U   $	��Top=�6i   ���	#DDF7F7F7$��15(�6�   $
��Bold"�6�   �� 5*   
   -   5        $ label_countCandidats��5*      $��Label��6.   $
��Left=�6>   $��
477,39,0,0q�6Y   $	��Top=�6m   ���	#DDFCF6F7$��15(�6�   $
��Bold"�6�   �� 5+   
   -   5        $ 
label_Time��5+      $��Czas :��6$   $
��Left=�65   $��208,410,0,0q�6P   $	��Top=�6e   $��14(�6}   $
��Bold"�6�   �� 5,   
   -   5        $ imageControl��5,      $
��Left=�6&   $	��316��6A   $��
189,79,0,0q�6N   $	��Top=�6b   $	��559��6z   $��Main_Images/helpGIF.gif��6�   �� 5-   
   -   5        $ btn_lupa��5-      $��695,411,3,0q�6#   $	��Top=�68   $��48��6�   $ 0.5,0.5&�6�   $ Powiększyć obraz ?��6�   ��6P   Z� ��6f   Z� ��6�   Z� .��5.      �� -   5        $ 	btn_lupa_��5.      $��Main_Images/btn_lupe.png��6'   $
��Fill=�6I   $ 0.892,0.842&�6X   $��23��6|   $��30��6�   $��2��6�   5/      �� 50   
   -   5        $ 
label_Copy��50      $��Znaki :��6$   $
��Left=�66   $��1,-4,0,0q�6Q   $	��Top=�6c   $��22(�6{   $
��Bold"�6�   �� 51   
   -   5        $ btn_AllSigns��51      $
��Left=�6'   $��
668,9,-5,0q�6B   $	��Top=�6V   $��88��6n   $��67��6�   $ 0.838,0.486&�6�   $- 'Wyświetl wszystkie rozpoznane znaki :)��6  ��6y   Z� ��6�   Z� ��6�   Z� .��52      �� $��Main_Images/btn_sign.png��6   $��44��66   $��52��6B   6O   53      �� 54   
   -   5        $ btn_candidats��54      $
��Left=�6(   $��	604,9,0,0q�6C   $	��Top=�6V   $��83��6n   $��67��6�   $ 0.838,0.486&�6�   $) #Wyświetl wszystkich kandydatów :)��6  ��6y   Z� ��6�   Z� ��6�   Z� .��55      �� $#��Main_Images/btn_candidats.png��6   $��44��6;   $��52��6G   56      �� 57   
   -   5        $ button_refresh��57      $
��Left=�6)   $��643,415,0,0q�6D   $	��Top=�6Y   $��65��6q   $��45��6|   $ Rozpoznaj ponownie��6�   ��6�   Z� ��6�   Z� .��58      �� -   5        $ image��58      $��37��6#   $��40��6/   $!��Main_Images/btn_refresh.png��6:   59      �� 5:   
   -   5         ��Name$ sliderVideo��5:      $
��Left=�6&   $��180,437,0,0q�6B   $	��Top=�6W   $	��138��6o   ���	#FFEFF7F6 � Maximum$	 100Q�6  5<      �� 5=   
   -   5        $ label_TimeTotal��5=      $��Label��6)   $
��Left=�69   $��243,445,0,0q�6T   $	��Top=�6i   $��9(�6�   ���	#DDF7F6F66�   �� 5>   
   -   5        $ button_maches��5>      $
��Left=�6(   $��587,416,0,0q�6C   $	��Top=�6X   $��73��6p   $��45��6{   $H BRysuj znalezione dopasowania punktów kluczowych z dwóch obrazów��6�   ��6�   Z� ��6  Z� .��5?      �� -   5        $ image1��5?      $��37��6$   $��40��60   $ ��Main_Images/btn_maches.png��6;   5@      � 5A   
   $
��Left=�6   $	��100��6*   $	��Top=�67   $	��100��6O   � 5B   
   $
��Left=�6   $	��100��6*   $��465,295,0,0q�67   $	��Top=�6L   $	��100��6d   $ 0.918,-0.032&�6p   5F      5G      