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
            Tundra = 0,       // 0000 0000
            Desert = 1,       // 0000 0001
            Plains = 2,       // 0000 0010
            Praire = 3,       // 0000 0011
            Grassland = 4,    // 0000 0100
            Savannah = 5,     // 0000 0101
            Marsh = 6,        // 0000 0110
            Swamp = 7,        // 0000 0111

            Forest = 8,       // 0000 1000

            Hill = 32,        // 0010 0000
            Mountain = 160,   // 1010 0000
            River = 64,       // 0100 0000
            MajorRiver = 192, // 1100 0000

            Arctic = 24,      // 0001 1000
            Ocean = 25,       // 0001 1001
            SeaLane = 26      // 0001 1010

        }


        static void Main(string[] args)
        {
            byte coordX;
            byte coordY;
            byte coordX2;
            byte coordY2;
            int cityType;

            Console.WriteLine("Укажите год файла сохранения:");
            int fileYear = int.Parse(Console.ReadLine());

            string path = @"C:\colonization(win)\SPA" + fileYear + ".SAV";

            // Выбор действия:
            // 1 - перенести город колонистов,
            // 2 - перенести поселение индейцев,
            // 3 - редактировать местность
            do {
                Console.WriteLine("Определите действие:\n1 - перенести город колонистов,\n2 - перенести поселение индейцев,\n3 - редактировать местность");
                cityType = int.Parse(Console.ReadLine());
                if (cityType > 0 && cityType <= 3)
                    break;
                else
                    Console.WriteLine("Неверный выбор, значение должно быть в пределах от 1 до 3");
            } while (true);


            // Ввод начальных координат
            coordX = MapCoord(true);
            coordY = MapCoord(false);


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
            // после отрядов идёт какая-то область в 1266 байтов
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

            long cityCoordOnTerrainMap = mapStart + coordY * 58 + coordX;
            long cityCoordOnInfraMap = cityCoordOnTerrainMap + 0x1050;

            Console.WriteLine("Город-T - " + cityCoordOnTerrainMap.ToString("X"));
            Console.WriteLine("Город-S - " + cityCoordOnInfraMap.ToString("X"));

            if (cityType == 1)
            {
                coordX2 = MapCoord(true);
                coordY2 = MapCoord(false);

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


            if (cityType == 2)
            {
                coordX2 = MapCoord(true);
                coordY2 = MapCoord(false);

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

            if (cityType == 3)
            {

                Console.WriteLine(cityCoordOnInfraMap.ToString("X"));
                Console.WriteLine(savFileArr[cityCoordOnInfraMap]);
                Console.WriteLine("Клетка содержит дорогу - " +
                    (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FRoad) == TerrainInfr.FRoad ? "Yes" : "No"));
                Console.WriteLine("{0,3} - {1}", savFileArr[cityCoordOnInfraMap],
                    ((TerrainInfr)savFileArr[cityCoordOnInfraMap]).ToString());
                if (((TerrainInfr)savFileArr[cityCoordOnInfraMap] & TerrainInfr.FRoad) != TerrainInfr.FRoad)
                    savFileArr[cityCoordOnInfraMap] += (byte)TerrainInfr.FRoad;
                Console.WriteLine(savFileArr[cityCoordOnInfraMap]);
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
        private static byte MapCoord(bool axis)
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

    }
}
