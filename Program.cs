using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO; // pour ReadAllBytes
using System.Diagnostics; // pour Process.Start()

namespace TD2A5
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
            
            Console.ReadKey();
        }

        private static void MainMenu()
        {
            ConsoleKeyInfo sortir; // pour associer touche escape au retour au menu
            ConsoleKeyInfo changer;

                ChangeImage: // label permettant un retour à ce point pour changer d'image à tout moment
            MyImage image1 = MenuImageSelection();
                          
            do
            {
                Console.Clear(); // to ensure the menu will be printed onto a blank console screen
                
                Console.WriteLine("Que souhaitez-vous faire avec cette image ?" + '\n');
                Console.WriteLine("1 - Informations sur l'image.");
                Console.WriteLine("2 - Noir et Blanc.");
                Console.WriteLine("3 - Nuance de Gris.");
                Console.WriteLine("4 - Effet Mirroir.");
                Console.WriteLine("5 - Rétrécissement.");
                Console.WriteLine("6 - Agrandissement.");
                Console.WriteLine("7 - Rotation 90, 180, 270.");
                Console.WriteLine("8 - Rotation tout angle.");
                Console.WriteLine("9 - Détection des contours.");
                Console.WriteLine("10 - Flou.");
                Console.WriteLine("11 - Renforcement de bords.");
                Console.WriteLine("12 - Repoussage.");
                Console.WriteLine("13 - Fractale de Mandelbrot (n'utilise pas l'image choisie).");
                Console.WriteLine("14 - Histogramme.");
                Console.WriteLine("15 - Codage d'une image.");
                Console.WriteLine("16 - Décodage d'une image cachée.");
                Console.WriteLine("17 - Explosion de couleurs (Innovation).");

                Console.Write("\r\nSélectionnez une option : ");


                switch (Console.ReadLine())
                {

                    case "1": // info sur l'image

                        Console.WriteLine(image1.toString());

                        break;

                    case "2": // noir et blanc

                        image1.BlackAndWhite();
                        image1.From_Image_To_File("sortieBlackAndWhite.bmp");

                        try
                        {
                            Process.Start("sortieBlackAndWhite.bmp");
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "3": // nuances de gris

                        image1.GrayScale();
                        image1.From_Image_To_File("sortieGrayScale.bmp");

                        try
                        {
                            Process.Start("sortieGrayScale.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "4": // effet miroir
                        image1.Mirror();
                        image1.From_Image_To_File("sortieMirror.bmp");

                        try
                        {
                            Process.Start("sortieMirror.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "5": // rétrécissement
                        Console.WriteLine('\n' + "Saisissez le coeffcicient de rétrécissement souhaité :");

                        image1.Smaller(Convert.ToInt32(Console.ReadLine()));
                        image1.From_Image_To_File("sortieSmaller.bmp");

                        try
                        {
                            Process.Start("sortieSmaller.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }
                    break;

                    case "6": // agrandissement
                        Console.WriteLine('\n' + "Saisissez le coeffcicient d'agrandissement souhaité :");

                        image1.Bigger(Convert.ToInt32(Console.ReadLine()));
                        image1.From_Image_To_File("sortieBigger.bmp");

                        try
                        {
                            Process.Start("sortieBigger.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "7": // rotation angle 90, 180, 270
                            ResaisieRotation:

                        Console.WriteLine("Saisir le degré de rotation souhaité (90, 180 ou 270) : ");

                        switch (Console.ReadLine())
                        {
                             case "90":
                                image1.Rotation90();
                                image1.From_Image_To_File("sortieRotation90.bmp");

                                try
                                {
                                    Process.Start("sortieRotation90.bmp");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                                }

                                break;

                            case "180":
                                image1.Rotation180();
                                image1.From_Image_To_File("sortieRotation180.bmp");

                                try
                                {
                                    Process.Start("sortieRotation180.bmp");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                                }

                                break;

                            case "270":
                                image1.Rotation270();
                                image1.From_Image_To_File("sortieRotation270.bmp");

                                try
                                {
                                    Process.Start("sortieRotation270.bmp");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                                }

                                break;

                            default:
                                Console.WriteLine("Ce degré de rotation ne peut pas être effectué." + '\n' + "Il faut saisir un degré de rotation parmi 90, 180 et 270.");
                                
                                goto ResaisieRotation;

                                break;
                        }
                        
                        break;

                    case "8": // rotation tout angle
                        Console.WriteLine('\n' + "Saisir l'angle de rotation souhaité : ");

                        image1.Rotation(Convert.ToInt32(Console.ReadLine()));

                        image1.From_Image_To_File("sortieRotation.bmp");

                        try
                        {
                            Process.Start("sortieRotation.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "9": // convo : détection des contours                                                
                        int[,] noyauContours = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };                        
                        double coeffAttenuation = 0.06; // atténue l'effet (filtre)
                        image1.Convolution(noyauContours, coeffAttenuation);
                        image1.From_Image_To_File("sortieContours.bmp");

                        try
                        {
                            Process.Start("sortieContours.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "10":  // convo : flou
                        int[,] noyauFlou = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                        double coeffAttenuation2 = 1;
                        image1.Convolution(noyauFlou, coeffAttenuation2); 
                        image1.From_Image_To_File("sortieFlou.bmp");

                        try
                        {
                            Process.Start("sortieFlou.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "11":  // convo : renforcement des bords / amélioration de la netteté
                        int[,] noyauNet = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };                        
                        double coeffAttenuation3 = 0.1;
                        image1.Convolution(noyauNet, coeffAttenuation3);
                        image1.From_Image_To_File("sortieNet.bmp");

                        try
                        {
                            Process.Start("sortieNet.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "12": // repoussage
                        int[,] noyauRep = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                        double coeffAttenuation4 = 0.85; // à retravailler
                        image1.Convolution(noyauRep, coeffAttenuation4);
                        image1.From_Image_To_File("sortieRepoussage.bmp");

                        try
                        {
                            Process.Start("sortieRepoussage.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "13": // Fractale Mandelbrot                        
                        MyImage.Mandelbrot();

                        try
                        {
                            Process.Start("sortieFractaleMandelbrot.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "14": // Histogramme
                        image1.Histogramme();
                        image1.From_Image_To_File("sortieHisto.bmp");

                        try
                        {
                            Process.Start("sortieHisto.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "15": // Coder une image dans une image
                        Console.WriteLine('\n' + "Il vous faut sélectionner une image à cacher dans votre image initiale.");
                        MyImage image2 = MenuImageSelection();
                        image1.CodageImage(image2);
                        image1.From_Image_To_File("sortieCodage.bmp");

                        try
                        {
                            Process.Start("sortieCodage.bmp");

                            Console.WriteLine('\n' + "Pour décoder cette image, changez d'image et saisissez le nom du fichier : sortieCodage.");
                            Console.WriteLine("Ensuite, sélectionnez l'option 16 : Décodage.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "16":  // décoder une image cachée                      
                        image1.DecodageImage();
                        image1.From_Image_To_File("sortieDecodage.bmp");

                        try
                        {
                            Process.Start("sortieDecodage.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;

                    case "17": // convo création
                        int[,] noyau1 = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } }; // + prononcé
                        int[,] noyau2 = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } }; // + léger
                        image1.Creation(noyau2);
                        image1.From_Image_To_File("sortieExplosion.bmp");

                        try
                        {
                            Process.Start("sortieExplosion.bmp");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("L'image n'a pas pu être affichée." + '\n' + e.Message);
                        }

                        break;


                    default: // si le numéro entré n'est pas dans le menu
                        Console.WriteLine("Cette option n'existe pas.");

                        break;
                }

                Console.WriteLine('\n' + "Tapez 'Entrée' pour revenir au menu des options, ou tapez 'C' pour changer d'image.");                

                changer = Console.ReadKey();

                if (changer.Key == ConsoleKey.C) // permet de changer l'image si la la lettre 'c' est entrée
                {
                    goto ChangeImage;
                }

                sortir = Console.ReadKey();

            } while (sortir.Key != ConsoleKey.Escape); // touche entrée = revenir au menu
        }

        public static MyImage MenuImageSelection() // menu pour sélectionner une image parmi les fichiers
        {
                ErreurLecture:
            try
            {                
                Console.WriteLine('\n' + "Veuillez saisir le nom d'un fichier (bitmap) parmi les noms suivant : " + '\n');
                Console.WriteLine("coco - lena - chat - emile - teemo - lac - carre" + '\n');
                MyImage image1 = new MyImage(Console.ReadLine() + ".bmp");

                return image1;
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur : Ce nom de fichier n'existe pas." + '\n' + "Tapez 'Entrée' pour resaisir le nom d'un fichier.");
                Console.ReadKey();
                goto ErreurLecture;
            }                        
        }

    }
}
