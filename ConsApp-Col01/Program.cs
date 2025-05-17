using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsApp_Col01
{
    class Program
    {
        static byte[] savFileArr; // Массив в который записываются данные из файла сохранения


        [Flags]
        enum TerrainInfr: byte
        {
            NullFlag = 0,   // 0000 0000

            FUnit = 1,      // 0000 0001
            FColony = 2,    // 0000 0010
            FUnkn3 = 4,     // 0000 0100
            FRoad  = 8,     // 0000 1000
            FCoast = 16,    // 0001 0000
            FUnkn6 = 32,    // 0010 0000
            FField = 64,    // 0100 0000
            FUnkn8 = 128    // 1000 0000
        }

        [Flags]
        enum TerrainType : byte
        {
            tTundra = 0x00,    // 0000 0000,    // 00 Tundra

            tDesert = 0x01,    // 0000 0001,    // 01 Desert
            tPlains = 0x02,    // 0000 0010,    // 02 Plains
            tPraire = 0x03,    // 0000 0011,    // 03 Praire
            tGrass  = 0x04,    // 0000 0100,    // 04 Grassland
            tSavan  = 0x05,    // 0000 0101,    // 05 Savannah
            tMarsh  = 0x06,    // 0000 0110,    // 06 Marsh
            tSwamp  = 0x07,    // 0000 0111,    // 07 Swamp

            tWaterL = 0x10,    // 0001 0000,    // 10 WaterLand Такая клетка не встречается

            tArctic = 0x18,    // 0001 1000,    // 18 Arctic
            tOcean  = 0x19,    // 0001 1001,    // 19 Ocean
            tSeaLan = 0x1A,    // 0001 1010,    // 1A Sea Lane

            tForest = 0x08,    // 0000 1000,    // 08 Boreal Forest (Tundra)
            tHill   = 0x20,    // 0010 0000,    // 20 Hill + Tundra
            tRiver  = 0x40,    // 0100 0000,    // 40 River + Tundra

            //           tOceanR = 01011001,    // 59 River + Ocean

            tMount  = 0xA0,    // 1010 0000,    // A0 Mountain + Tundra
            tMRiver = 0xC0     // 1100 0000     // C0 Major River + Tundra

            //           tOceanMR =11011001     // D9 Major River + Ocean
        }


        static void Main(string[] args)
        {
            byte coordX;
            byte coordY;
            byte coordX2;
            byte coordY2;
            int choise;

            Console.WriteLine("Укажите год файла сохранения:");
            int fileYear = int.Parse(Console.ReadLine());

            string path = @"C:\colonization(win)\SPA" + fileYear + ".SAV";

            // Выбор действия:
            // 1 - перенести город колонистов,
            // 2 - перенести поселение индейцев,
            // 3 - редактировать местность
            do {
                Console.WriteLine("Определите действие:\n1 - перенести город колонистов,\n2 - перенести поселение индейцев,\n3 - редактировать местность");
                choise = int.Parse(Console.ReadLine());
                if (choise > 0 && choise <= 3)
                    break;
                else
                    Console.WriteLine("Неверный выбор, значение должно быть в пределах от 1 до 3");
            } while (true);


            // Ввод начальных координат
            coordX = MapCoordInput(true);
            coordY = MapCoordInput(false);


            // чтение файла
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {

                    BinaryReader binReader = new BinaryReader(fs);

                    savFileArr = new byte[fs.Length];
                    binReader.Read(savFileArr, 0, (int)savFileArr.Length);
                }
            }

            // со смещения 0x186 - начинаются города
            // by[0x2E] - 46i - кол-во городов (сведения об одном городе - 202 байта (CAh))
            //
            // by[0x2C] - 44i - кол-во отрядов (сведения об одном отряде - 28 байт (1Сh))
            //
            // после отрядов идёт какая-то область в 1266 байт
            // by[0x2A] - 42i - кол-во индейских поселений (сведения об одном поселении - 18 байт (1Сh))
            // после индейских поселений идёт какая-то область в 1349 байтов


            //Console.WriteLine("Кол-во отрядов - " + cUnit);

            int mapStart = 0x186 + savFileArr[0x2E] * 202 + BitConverter.ToInt16(savFileArr, 0x2C) * 28 + 1266 + savFileArr[0x2A] * 18 + 1349;
            Console.WriteLine("Старт карты - " + mapStart.ToString("X"));

            //int cUnit = BitConverter.ToInt16(by, 0x2C);
            //int movCity = 0x186 + by[0x2E] * 202;
            //int movUnit = 0x186 + by[0x2E] * 202 + cUnit * 28;
            //int movCamp = 0x186 + by[0x2E] * 202 + cUnit * 28 + 1266 + by[0x2A] * 18;
            //Console.WriteLine("кол-во городов - " + by[0x2E].ToString());
            //Console.WriteLine("Смещение городов - " + movCity.ToString("X"));
            //Console.WriteLine("кол-во отрядов - " + cUnit);
            //Console.WriteLine("Смещение отрядов - " + movUnit.ToString("X"));
            //Console.WriteLine("кол-во индейских поселений - " + by[0x2A].ToString());
            //Console.WriteLine("Смещение индейских поселений - " + movCamp.ToString("X"));

            // Координата на карте рельефа
            long cityCoordOnTerrainMap = mapStart + coordY * 58 + coordX;
            // Координата на карте инфраструктуры
            long cityCoordOnInfraMap = cityCoordOnTerrainMap + 0x1050;

            Console.WriteLine("Город-T - " + cityCoordOnTerrainMap.ToString("X"));
            Console.WriteLine("Город-S - " + cityCoordOnInfraMap.ToString("X"));


            // перенести город колонистов
            if (choise == 1)
            {
                coordX2 = MapCoordInput(true);
                coordY2 = MapCoordInput(false);

                long cityCoordOnTerrainMap2 = mapStart + coordY2 * 58 + coordX2;
                long cityCoordOnInfraMap2 = cityCoordOnTerrainMap2 + 0x1050;

                // описать область городов от и до
                for (int i = 0x186; i < (0x186 + savFileArr[0x2E] * 202 - 1); i++)
                {
                    if (savFileArr[i] == coordX && savFileArr[i + 1] == coordY)
                    {
                        // установка новых координат в координаты города
                        savFileArr[i] = coordX2;
                        savFileArr[i + 1] = coordY2;
                    }
                }

                // удаление информации по старому местоположению
                if (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FColony) == TerrainInfr.FColony)
                {
                    savFileArr[cityCoordOnInfraMap] -= (byte)TerrainInfr.FColony;
                    savFileArr[cityCoordOnInfraMap2] += (byte)TerrainInfr.FColony;

                    //смена значений
                    byte temp = savFileArr[cityCoordOnInfraMap + 0x1050];
                    savFileArr[cityCoordOnInfraMap + 0x1050] = savFileArr[cityCoordOnInfraMap2 + 0x1050];
                    savFileArr[cityCoordOnInfraMap2 + 0x1050] = temp;

                }
            }


            // перенести поселение индейцев
            if (choise == 2)
            {
                coordX2 = MapCoordInput(true);
                coordY2 = MapCoordInput(false);

                long cityCoordOnTerrainMap2 = mapStart + coordY2 * 58 + coordX2;
                long cityCoordOnInfraMap2 = cityCoordOnTerrainMap2 + 0x1050;

                // со смещения 0x186 - начинаются города
                // by[0x2E] - 46i - кол-во городов (сведения об одном городе - 202 байта (CAh))
                //
                // by[0x2C] - 44i - кол-во отрядов (сведения об одном отряде - 28 байт (1Сh))
                //
                // после отрядов идёт какая-то область в 1266 байтов
                // by[0x2A] - 42i - кол-во индейских поселений (сведения об одном поселении - 18 байт (1Сh))
                // после индейских поселений идёт какая-то область в 1349 байтов

                // описать область индейских поселений
                int startIndianWillage = 0x186 + savFileArr[0x2E] * 202 + BitConverter.ToInt16(savFileArr, 0x2C) * 28 + 1266;
                for (int i = startIndianWillage; i < (startIndianWillage + savFileArr[0x2A] * 18 - 1); i++)
                {
                    if (savFileArr[i]   == coordX && savFileArr[i + 1] == coordY)
                    {
                        // установка новых координат в координаты города
                        savFileArr[i] = coordX2;
                        savFileArr[i + 1] = coordY2;

                        Console.WriteLine("Начало поселений индейцев - " + startIndianWillage);
                        Console.WriteLine("Конец поселений индейцев - " + (startIndianWillage + savFileArr[0x2A] * 18 - 1));
                    }
                }

                // удаление информации по старому местоположению
                if (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FColony) == TerrainInfr.FColony)
                {
                    //проверка, есть ли в деревне отряды
                    if (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FUnit) == TerrainInfr.FUnit)
                    {
                        // поиск индейского отряда находящегося в деревне
                        int startIndianUnit = 0x186 + savFileArr[0x2E] * 202;

                        //Смена координат у индейского отряда.
                        for (int i = startIndianUnit; i < (startIndianUnit + BitConverter.ToInt16(savFileArr, 0x2C) * 28 - 1); i++)
                        {
                            if (savFileArr[i] == coordX && savFileArr[i + 1] == coordY)
                            {
                                // установка новых координат в координаты города
                                savFileArr[i] = coordX2;
                                savFileArr[i + 1] = coordY2;
                            }
                        }

                        //смена значений на слое инфраструктуры
                        savFileArr[cityCoordOnInfraMap] -= (byte)TerrainInfr.FUnit;
                        savFileArr[cityCoordOnInfraMap2] += (byte)TerrainInfr.FUnit;
                    }

                    //смена значений на слое инфраструктуры
                    savFileArr[cityCoordOnInfraMap] -= (byte)TerrainInfr.FColony;
                    savFileArr[cityCoordOnInfraMap2] += (byte)TerrainInfr.FColony;

                    //смена значений на слое отрядов
                    byte temp = savFileArr[cityCoordOnInfraMap + 0x1050];
                    savFileArr[cityCoordOnInfraMap + 0x1050] = savFileArr[cityCoordOnInfraMap2 + 0x1050];
                    savFileArr[cityCoordOnInfraMap2 + 0x1050] = temp;

                }
            }


            // редактировать местность
            if (choise == 3)
            {

                Console.WriteLine(cityCoordOnInfraMap.ToString("X"));
                Console.WriteLine(savFileArr[cityCoordOnInfraMap]);
                Console.WriteLine("Клетка содержит дорогу - " +
                    (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FRoad) == TerrainInfr.FRoad ? "Yes" : "No"));
                Console.WriteLine("Клетка содержит поля - " +
                    (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FField) == TerrainInfr.FField ? "Yes" : "No"));
                Console.WriteLine();
                //Console.WriteLine("{0,3} - {1}", savFileArr[cityCoordOnInfraMap],
                //    ((TerrainInfr)savFileArr[cityCoordOnInfraMap]).ToString());

                //    tWaterL = 0x10,    // 0001 0000,    // 10 WaterLand Такая клетка не встречается
                //    tArctic = 0x18,    // 0001 1000,    // 18 Arctic
                //    tOcean = 0x19,     // 0001 1001,    // 19 Ocean
                //    tSeaLan = 0x1A,    // 0001 1010,    // 1A Sea Lane
                //    tOceanR = 0x59     // 0101 1001,    // 59 River + Ocean
                //    tOceanMR = 0xD9    // 1101 1001     // D9 Major River + Ocean

                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tWaterL) == TerrainType.tWaterL)
                {
                    if (savFileArr[cityCoordOnTerrainMap] == 0x18)
                        Console.WriteLine("Arctic");

                    else if (savFileArr[cityCoordOnTerrainMap] == 0x1A)
                        Console.WriteLine("Sea Lane");

                    else if (savFileArr[cityCoordOnTerrainMap] == 0x19)
                        Console.WriteLine("Ocean");

                    else if(savFileArr[cityCoordOnTerrainMap] == 0x59)
                        Console.WriteLine("Ocean + River");

                    else if(savFileArr[cityCoordOnTerrainMap] == 0xD9)
                        Console.WriteLine("Ocean + Major River");

                }

                //    tTundra = 0x00,    // 0000 0000,    // 00 Tundra
                //    tDesert = 0x01,    // 0000 0001,    // 01 Desert
                //    tPlains = 0x02,    // 0000 0010,    // 02 Plains
                //    tPraire = 0x03,    // 0000 0011,    // 03 Praire
                //    tGrass = 0x04,     // 0000 0100,    // 04 Grassland
                //    tSavan = 0x05,     // 0000 0101,    // 05 Savannah
                //    tMarsh = 0x06,     // 0000 0110,    // 06 Marsh
                //    tSwamp = 0x07,     // 0000 0111,    // 07 Swamp
                //    tForest = 0x08,    // 0000 1000,    // 08 Forest
                //    tHill = 0x20,      // 0010 0000,    // 20 Hill + Tundra
                //    tRiver = 0x40,     // 0100 0000,    // 40 River + Tundra
                //    tMount = 0xA0,     // 1010 0000,    // A0 Mountain + Tundra
                //    tMRiver = 0xC0     // 1100 0000     // C0 Major River + Tundra
                else
                {
                    if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tSwamp) == TerrainType.tSwamp)
                        Console.Write("Swamp");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tMarsh) == TerrainType.tMarsh)
                        Console.Write("Marsh");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tSavan) == TerrainType.tSavan)
                        Console.Write("Savannah");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tGrass) == TerrainType.tGrass)
                        Console.Write("Grassland");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tPraire) == TerrainType.tPraire)
                        Console.Write("Praire");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tPlains) == TerrainType.tPlains)
                        Console.Write("Plains");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tDesert) == TerrainType.tDesert)
                        Console.Write("Desert");

                    else 
                        Console.Write("Tundra");


                    if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tMRiver) == TerrainType.tMRiver)
                        Console.Write(" + Major River");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tMount) == TerrainType.tMount)
                        Console.Write(" + Mount");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tRiver) == TerrainType.tRiver)
                        Console.Write(" + River");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tHill) == TerrainType.tHill)
                        Console.Write(" + Hill");

                    else if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tForest) == TerrainType.tForest)
                        Console.Write(" + Forest");
                }

                Console.WriteLine();

                // Выбор действия:
                // 1 - добавить / удалить дорогу,
                // 2 - добавить / удалить поле,
                // 3 - добавить / удалить лес,
                // 4 - редактировать местность
                do
                {
                    Console.WriteLine(@"Выбор действия:
1 - добавить / удалить дорогу,
2 - добавить / удалить поле,
3 - добавить / удалить лес,
4 - редактировать местность");
                    choise = int.Parse(Console.ReadLine());
                    if (choise > 0 && choise <= 3)
                        break;
                    else
                        Console.WriteLine("Неверный выбор, значение должно быть в пределах от 1 до 4");
                } while (true);

                if (choise == 1)
                    RoadBE(cityCoordOnTerrainMap);

                if (choise == 2)
                    FieldBE(cityCoordOnTerrainMap);

                if (choise == 3)
                    ForestBE(cityCoordOnTerrainMap);

                if (choise == 4)
                    RoadBE(cityCoordOnTerrainMap);

                Console.WriteLine();

            }


            // Сохранение файла
            using (FileStream fs = new FileStream(path + "2", FileMode.OpenOrCreate))
            {
                // Create the writer for data.
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    // Write data to Test.data.
                    for (int i = 0; i < savFileArr.Length; i++)
                    {
                        bw.Write(savFileArr[i]);
                    }
                }
            }

            Console.WriteLine("Программа завершена, нажмите любую клавишу.");
            Console.ReadLine();
        }


        /// <summary>
        /// Метод выдаёт запрос о вводе координат X - true, Y - false
        /// </summary>
        /// <param name="axis">X - true, Y - false</param>
        /// <returns></returns>
        private static byte MapCoordInput(bool axis)
        {
            byte coord;
            byte coordRange;
            char coordString;

            if (axis)
            {
                coordRange = 58;
                coordString = 'X';
            }
            else
            {
                coordRange = 70;
                coordString = 'Y';
            }

            do
            {
                Console.WriteLine("Укажите координату " + coordString);
                coord = byte.Parse(Console.ReadLine());
                if (coord > 0 && coord <= coordRange)
                    break;
                else
                    Console.WriteLine("Неверный диапазон, координата должен быть в пределах от  1 до " + coordRange);

            } while (true);

            return coord;
        }

        /// <summary>
        /// Добавление дороги если нет или удаление дороги если есть.
        /// </summary>
        /// <param name="cityCoordOnInfraMap">Координата на карте рельефа</param>
        private static void RoadBE(long cityCoordOnInfraMap)
        {
            if (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FRoad) != TerrainInfr.FRoad)
                savFileArr[cityCoordOnInfraMap] += (byte)TerrainInfr.FRoad;
            else
                savFileArr[cityCoordOnInfraMap] -= (byte)TerrainInfr.FRoad;
        }


        /// <summary>
        /// Добавление леса и удаление поля/холма/горы если нет леса или удаление леса если есть.
        /// </summary>
        /// <param name="cityCoordOnTerrainMap">Координата на карте рельефа</param>
        private static void ForestBE(long cityCoordOnTerrainMap)
        {
            if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tForest) != TerrainType.tForest)
            {
                savFileArr[cityCoordOnTerrainMap] += (byte)TerrainType.tForest;
                if (((TerrainInfr)savFileArr[cityCoordOnTerrainMap + 0x1050] & TerrainInfr.FField) == TerrainInfr.FField)
                    savFileArr[cityCoordOnTerrainMap + 0x1050] -= (byte)TerrainInfr.FField;
                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tMount) == TerrainType.tMount)
                    savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tMount;
                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tHill) == TerrainType.tHill)
                    savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tHill;
            }
            else
                savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tForest;
        }


        /// <summary>
        /// Добавление поля и удаление леса/холма/горы если нет поля или удаление поля если есть.
        /// </summary>
        /// <param name="cityCoordOnTerrainMap">Координата на карте рельефа</param>
        private static void FieldBE(long cityCoordOnTerrainMap)
        {
            if (((TerrainInfr)savFileArr[cityCoordOnTerrainMap + 0x1050] & TerrainInfr.FField) != TerrainInfr.FField)
            {
                savFileArr[cityCoordOnTerrainMap + 0x1050] += (byte)TerrainInfr.FField;
                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tForest) == TerrainType.tForest)
                    savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tForest;
                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tMount) == TerrainType.tMount)
                    savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tMount;
                if (((TerrainType)savFileArr[cityCoordOnTerrainMap] & TerrainType.tHill) == TerrainType.tHill)
                    savFileArr[cityCoordOnTerrainMap] -= (byte)TerrainType.tHill;

            }
            else
                savFileArr[cityCoordOnTerrainMap + 0x1050] -= (byte)TerrainInfr.FField;
        }
    }
}
