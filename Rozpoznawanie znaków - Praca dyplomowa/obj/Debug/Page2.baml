   M S B A M L   `   `   `  ����b  ^Rozpoznawanie znaków - Praca dyplomowa, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Rozpoznawanie_znaków___Praca_dyplomowa'Rozpoznawanie_znaków___Praca_dyplomowa  5      3    -Rozpoznawanie_znaków___Praca_dyplomowa.Page2   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      J FXamlAnimatedGif, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null<gif2https://github.com/XamlAnimatedGif/XamlAnimatedGif  5      Alocal5clr-namespace:Rozpoznawanie_znaków___Praca_dyplomowa  5        N� Title$  Page2��5	      $	��754��6   $	��476��6!   .��5
      � $��
0,-1,2.6,6q�6   ��5   
   �� 5       �� Color# � ��6   .+�5   
   �� -   5         ��Name$ btn_imageBox��5      $
��Left=�6&   $	��352��6A   $��	22,24,0,0q�6N   $	��Top=�6a   $	��682��6y   $
��Fill=�6�   �� 5   
   -   5         ��Name$ btn_back��5      $��Cofnij��6#   $
��Left=�64   $��
18,414,0,0q�6O   $	��Top=�6c   $	��142��6{   ���	#FF41B73A���	#FF41B73A$��43��6�    J� RenderTransformOrigin$ 0.838,0.486&�6�   �� 5   
   -   5        $ 	btn_Start��5      $��Start��6$   $
��Left=�64   $��591,411,0,0q�6O   $	��Top=�6d   $	��139��6|   ���	#FF41B73A���	#FF41B73A$��48��6�   $ 0.838,0.486&�6�   �� 5   
   -   5        $ image��5      $
��Left=�6   $	��100��6:   $��298,145,0,0q�6G   $	��Top=�6\   $	��100��6t   �� 5   
   -   5        $ img_VideoIcon��5      $
��Left=�6'   $	��100��6B   $��314,131,0,0q�6O   $	��Top=�6d   $	��100��6|   $ ��Main_Images/icon_video.png��6�   $y�Hidden=�6�   �� 5   
   '  !XamlAnimatedGif.AnimationBehavior   	SourceUri$ Main_Images\welcome.gif?�6   $��183,384,435,12q�5      $
��Fill=�6    �� 5   
   -   5         ��Name$ label_MethodSerach��5      $��Metoda Wyszukiwania��6,   $
��Left=�6J   $��387,407,0,0q�6e   $	��Top=�6z   $
��Bold"�6�   ���	#DD6200CA$��13(�6�   �� 5   
   -   5        $
 line��5      $
��Left=�6   $��14��69   $��
455,0,0,28q�6E   $��Bottom=�6Y   $��3��6t   $��Main_Images/line.png��6~   �� 5   
   -   5        $ lb_SURF��5      $��Kolor��6!   $
��Left=�61   $	��Top=�6L   $��11(�6d   $ -0.818,0.672&�6r   $��550,438,0,0q�6�   �� 5   
   -	   5        $ lb_SURF_Copy��5      $��Kształt��6&   $
��Left=�68   $	��Top=�6S   $��11(�6k   $ -0.818,0.672&�6y   $��324,439,0,0q�6�   �� 5   
   -
   5        $ lb_SURF_Copy1��5      $
��Left=�6'   $	��Top=�6B   $��11(�6Z   $ -0.818,0.672&�6h   $��433,439,0,0q�6�   �� 5   
   -   5        $ color_Box_Colors��5      $
��Left=�6*   $��35��6E   $��558,429,0,0q�6Q   $	��Top=�6f   $��20��6~   $ 1.167,-0.343&�6�   $��Main_Images/colors.png��6�   �� 5   
   -   5        $ color_Box_Shapes��5      $
��Left=�6*   $��37��6E   $��332,423,0,0q�6Q   $	��Top=�6f   $��26��6~   $ 1.167,-0.343&�6�   $��Main_Images/shapes.png��6�   �� 5   
   -   5         ��Name$ sliderMethodSerach��5      $
��Left=�6-   $	��Top=�6H   $��28��6`   $	��197��6l    � Maximum$ 3Q�6x   	 � Minimum$	 1Q�6�   
 � SmallChange$
 1Q�6�   
������� � Value$ 4Q�6�    � ToolTip$W QWybierz sposób wyszukiwania znaków drogowych na obrazie (np. znaki okrągłe). ��6�   $��360,429,0,0q�6[  ��6�   Z� 6[  �� 5   
   -   5        $ lb_all��5      $��	Wszystkie��6    $
��Left=�64   $	��Top=�6O   $��11(�6g   $ -0.818,0.672&�6u   $��429,436,0,0q�6�   5      5      