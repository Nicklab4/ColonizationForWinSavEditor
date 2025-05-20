# ColonizationForWinSavEditor
Editor for SAV files from "Colonization" for Windows

Ширина экрана в пикселах
1, 2  // int16

Высота экрана в пикселах
3, 4  // int16

Длинна "слова" которым закодирован цвет
5     // byte8

Размер палитры
6     // byte8

Палитра цветов
от 7 до (6+1)*3
Растровые данные




coldata8.dll res 101 - королевская лилия (спрайт)
82642F - E9
776347 - EA
675337 - EB




Первые 4 байта перед именем колонии - координаты колонии
33  30  53 74 2E 20 4C 6F 75 69 73
3   0   S  t  .     L  o  u  i  s
51i 48i

Этим же координатам должен соответствовать бит на карте инфраструктуры.

Блок индейских поселений
Первые 4 байта перед именем колонии - координаты поселения

Индейское поселение (сведения об одном поселении - 18 байт (12h))
01- coord X
02- coord Y
03-
04-
05- кол-во атак которые может выдержать поселение




Карта
57 по горизонатали - 58(i) 3A(h) 
70 по вертикали




Карта местности

00000000	00 Tundra
00000001	01 Desert
00000010	02 Plains
00000011	03 Praire
00000100	04 Grassland
00000101	05 Savannah
00000110	06 Marsh
00000111	07 Swamp

00001000	08 Boreal Forest (Tundra)
00001001	09 Scrub Forest (Desert)
00001010	0A Mixed Forest (Plains)
00001011	0B Broadleaf Forest (Praire)
00001100	0C Conifer Forest (Grassland)
00001101	0D Tropical Forest (Savannah)
00001110	0E Wetland Forest (Marsh)
00001111	0F Rain Forest(Swamp)

00011000	18 Arctic
00011001	19 Ocean
00011010	1A Sea Lane

00100000	20 Hill + Tundra
00100001	21 Hill + Desert
00100010	22 Hill + Plains
00100011	23 Hill + Praire
00100100	24 Hill + Grassland
00100101	25 Hill + Savannah
00100110	26 Hill + Marsh
00100111	27 Hill + Swamp

01000000	40 River + Tundra
01000001	41 River + Desert
01000010	42 River + Plains
01000011	43 River + Praire
01000100	44 River + Grassland
01000101	45 River + Savannah
01000110	46 River + Marsh
01000111	47 River + Swamp

01001000	48 River + Boreal Forest (Tundra)
01001001	49 River + Scrub Forest (Desert)
01001010	4A River + Mixed Forest (Plains)
01001011	4B River + Broadleaf Forest (Praire)
01001100	4C River + Conifer Forest (Grassland)
01001101	4D River + Tropical Forest (Savannah)
01001110	4E River + Wetland Forest (Marsh)
01001111	4F River + Rain Forest(Swamp)

01011001	59 River + Ocean

10100000	A0 Mountain + Tundra
10100001	A1 Mountain + Desert
10100010	A2 Mountain + Plains
10100011	A3 Mountain + Praire
10100100	A4 Mountain + Grassland
10100101	A5 Mountain + Savannah
10100110	A6 Mountain + Marsh
10100111	A7 Mountain + Swamp

11000000	C0 Major River + Tundra
11000001	C1 Major River + Desert
11000010	C2 Major River + Plains
11000011	C3 Major River + Praire
11000100	C4 Major River + Grassland
11000101	C5 Major River + Savannah
11000110	C6 Major River + Marsh
11000111	C7 Major River + Swamp

11001000	C8 Major River + Boreal Forest (Tundra)
11001001	C9 Major River + Scrub Forest (Desert)
11001010	CA Major River + Mixed Forest (Plains)
11001011	CB Major River + Broadleaf Forest (Praire)
11001100	CC Major River + Conifer Forest (Grassland)
11001101	CD Major River + Tropical Forest (Savannah)
11001110	CE Major River + Wetland Forest (Marsh)
11001111	CF Major River + Rain Forest(Swamp)

11011001	D9 Major River + Ocean

1050(h) - 4176(i) - смещение карты инфраструктуры относительно карты местности




Карта инфраструктуры
+--------- 8й бит - 
|
|+-------- 7й бит - флаг поля
||
||+------- 6й бит - бит тихого океана
|||
|||+------ 5й бит - клетка занята колонией
||||
||||+----- 4й бит - дорога
|||||
|||||+---- 3й бит - 
||||||
||||||+--- 2й бит - colony flag
|||||||
|||||||+-- 1й бит - unit flag
||||||||
11011001




Карта отношений
+--------- 8й бит - голландия
|
|+-------- 7й бит - испания
||
||+------- 6й бит - 
|||
|||+------ 5й бит - 
||||
||||+----- 4й бит - 
|||||
|||||+---- 3й бит - Lake
||||||
||||||+--- 2й бит - Land
|||||||
|||||||+-- 1й бит - Ocean
||||||||
11011001


0000     (0i) анг
0001     (1i) фр
0010     (2i) исп
0011     (3i) гол


0100     (4i) Inca
0101     (5i) Azteck
0110     (6i) aravac
0111     (7i) irocious
1000     (8i) Cherokee
1001     (9i) apache
1010     (10i) siu
1011     (11i) Tupi

1111     (15i) wilderness

00100010 Spain
00010010 France
00100001 spain ocean
00000001 ang ocean
11110001 ocean


Карта Видимости
+--------- 8й бит - голландия
|
|+-------- 7й бит - испания
||
||+------- 6й бит - 
|||
|||+------ 5й бит - берег 
||||
||||+----- 4й бит - дорога
|||||
|||||+---- 3й бит - 
||||||
||||||+--- 2й бит - colony flag
|||||||
|||||||+-- 1й бит - unit flag
||||||||
11011001



20h - 0 - 1 переход хода? 0 можно ходить 1 нельзя

22h - 88 - 91

2Ah - 42i - Количество индейских поселений (сведения об одном поселении - 18 байт (12h))

2C-2Dh - 16int - кол-во отрядов (сведения об одном отряде - 28 байт (1Сh))

77h 119i  -120i

2Eh - 46i - кол-во городов (сведения об одном городе - 202 байта (CAh))



186h - начинаются города

после городов идут отряды

после отрядов идёт информация о игроках
4F2h - 1266i /4 = 316i на игрока

1410h
142Ch - 1Ch 28i



01- 31h - 49i coord X
02- 26h - 38i coord Y
03- 04h
04- 42h - принадлежность стране первые 4 бита
		00h англия
		01h франция
		02h испания
		03h голландия
		04h inca
		05h ацтеки
		06h араваки
		07h ирокезы
		08h чероки
		09h апачи
		0Ah Сиу
		0Bh Тупи
		0Ch ??
		0Dh xxxx
		
05- 00h - Тип отряда
		00h Уголовник
		01h Уголовник солдат
		02h Уголовник Пионер 236 тоолс
		03h Уголовник Миссионер
		04h Уголовник Драгун
		05h Уголовник Скаут
		06h Регулярный солдат
		07h Уголовник Драгун конт. армии
		08h Регулярная Каваллерия
		09h Уголовник Солдат конт. армии
		0Ah Сокровище 2600
		0Bh Артиллерия
		0Ch Вагон
		0Dh Каравелла
		0Eh торговое судно
		0Fh Галеон
		10h пиратский корабль
		11h фрегат
		12h Мановар
		13h Бравес
		14h Вооруженный Бравес
		15h Бравес на коне
		16h Конный инд воин
06- 0Bh - Количество сделанных ходов. Отряды с 1м ходом = 3i, отряды с 2мя ходами = 6i, отряды с 4мя ходами = 12i (0Ch)
07- 00
08- 00
09- 00 - Статус отряда 01 Сентри 02 -T 03 -GoTo координаты
10- coord X
11- coord Y
24- 






