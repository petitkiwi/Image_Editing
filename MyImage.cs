using System;
using System.IO; // pour ReadAllBytes et File.WriteAllBytes
using System.Numerics;

namespace TD2A5
{
    class MyImage
    {
        // attributs
        static string type = "Bitmap";
        int tailleFichier;
        int offset;
        int largeur;
        int hauteur;
        int bitsParCouleur;
        byte[] tabImage;
        Pixel[,] image;

        // constructeur
        public MyImage(string myfile)
        {           
            byte[] imagefile = File.ReadAllBytes(myfile); // récup l'image dans les fichiers debug

            this.tailleFichier = Convertir_Endian_To_Int(imagefile, 2, 5);
            this.offset = Convertir_Endian_To_Int(imagefile, 10, 13);
            this.largeur = Convertir_Endian_To_Int(imagefile, 18, 21);
            this.hauteur = Convertir_Endian_To_Int(imagefile, 22, 25);
            this.bitsParCouleur = Convertir_Endian_To_Int(imagefile, 28, 29);
            this.tabImage = imagefile;
            this.image = MatriceRGB(imagefile);           
        }

        // propriétés
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public int TailleFichier
        {
            get { return tailleFichier; }
            set { tailleFichier = value; }
        }
        public int Offset
        {
            get { return offset; }
        }
        public int Largeur
        {
            get { return largeur; }
            set { largeur = value; }
        }
        public int Hauteur
        {
            get { return hauteur; }
            set { hauteur = value; }
        }
        public int BitsParCouleur
        {
            get { return bitsParCouleur; }
        }
        public Pixel[,] Image
        {
            get { return image; }
            set { image = value; }
        }

        /// <summary>
        /// Méthode convertissant un tableau de bits little endian en une valeur entière
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab, int startIndex, int endIndex)
        {
            int conversion = 0;
            int k = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                conversion += tab[i] * (int)Math.Pow(256, k); // fonction puissance, base 256, somme des tab[i]*256^k 
                k++;
            }
            return conversion;
        }

        /// <summary>
        /// Méthode convertissant une valeur entière en un tableau de little endian
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] conversion = new byte[4];
            int quotient;
            int k = 0;

            while ((int)Math.Pow(256, k) <= val) // regarder la plus grande puissance de 256 avec laquelle on peut diviser val
            {
                k++;
            }

            if (k > 0)
            {
                k--;

                for (k = k; k > 0; k--)
                {
                    quotient = val / ((int)Math.Pow(256, k));
                    conversion[k] = Convert.ToByte(quotient);
                    val = val % ((int)Math.Pow(256, k));
                }

                conversion[k] = Convert.ToByte(val);
            }
            else
            {
                conversion[0] = Convert.ToByte(val);

                for (int i = 1; i < conversion.Length; i++)
                {
                    conversion[i] = 0;
                }
            }

            return conversion;
        }

        /// <summary>
        /// Méthode qui prend une instance de MyImage et la transforme en fichier binaire (transforme matrice modifiée en une new image)
        /// </summary>
        /// <param name="sortie"></param>
        public void From_Image_To_File(string sortie) 
        {
            byte[] editedImage = new byte[offset + (hauteur * largeur * 3)];

            for (int i = 0; i < offset; i++) // les informations sur l'image changent si la taille est modifiée
            {
                editedImage[i] = tabImage[i];
            }

            // taille fichier
            for (int i = 2; i <= 5; i++)
            {
                editedImage[i] = Convertir_Int_To_Endian(tailleFichier)[i - 2];
            }

            // largeur & hauteur
            for (int i = 18; i <= 21; i++)
            {
                editedImage[i] = Convertir_Int_To_Endian(largeur)[i - 18];
            }
            for (int i = 22; i <= 25; i++)
            {
                editedImage[i] = Convertir_Int_To_Endian(hauteur)[i - 22];
            }

            int k = offset;

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    editedImage[k] = image[i, j].Red;
                    editedImage[k + 1] = image[i, j].Green;
                    editedImage[k + 2] = image[i, j].Blue;

                    k += 3;
                }
            }

            File.WriteAllBytes(sortie, editedImage); // enregistre l'image dans debug, dont le nom sera : sortie
        }

        /// <summary>
        /// Cette méthode me permet d'afficher la matrice de pixels d'une image, elle me servait surtout en cas de recherche d'erreur au début du projet
        /// </summary>
        /// <param name="matrice"></param>
        /// <returns></returns>
        static string AfficherMatrice(Pixel[,] matrice)
        {
            string mat = "";

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    mat += Convert.ToString(matrice[i, j].Red) + ' ' + Convert.ToString(matrice[i, j].Green) + ' ' + Convert.ToString(matrice[i, j].Blue) + ' ';
                }
                mat += '\n';
            }

            return mat;

        }

        /// <summary>
        /// Cette méthode donne une description des informations d'une image
        /// </summary>
        /// <returns></returns>
        public string toString() // info sur l'image
        {
            string description = '\n' + "Quelques informations sur l'image :" + '\n';
            description += "Taille du fichier : " + tailleFichier + '\n';
            description += "Type : " + type + '\n' + "Offset (décalage avant d'atteindre le premier octet de l'image) : " + offset + '\n';
            description += "Taille de l'image : " + largeur + "x" + hauteur + '\n';
            description += "Nombre de bits par couleur : " + bitsParCouleur + '\n';
            // description += "Matrice de pixels : " + '\n' + AfficherMatrice(image); // lag console si image trop grande

            return description;
        }

        /// <summary>
        /// Cette méthode convertit l'image elle-même (byte) en une matrice de pixels (R,G,B) afin de faire des opérations sur la matrice et ainsi modifier l'image      
        /// </summary>
        /// <param name="imagefile"></param>
        /// <returns></returns>
        public Pixel[,] MatriceRGB(byte[] imagefile)
        {
            Pixel[,] matImage = new Pixel[hauteur, largeur];

            int k = 0;

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matImage[i, j] = new Pixel(tabImage[offset + k], tabImage[offset + k + 1], tabImage[offset + k + 2]); // on en prend que les info de l'image elle-même, soit après l'offset

                    k += 3;
                }
            }

            return matImage;

        }

        /// <summary>
        /// Méthode permettant de mettre l'image en noir et blanc
        /// </summary>
        public void BlackAndWhite()
        {
            byte moyenne;

            try
            {
                for (int i = 0; i < hauteur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        moyenne = (byte)((image[i, j].Red + image[i, j].Green + image[i, j].Blue) / 3);

                        if (moyenne < 128) // imposons un seuil (128) au dessous duquel le pixel devient noir et au dessus duquel il devient blanc
                        {
                            image[i, j].Red = 0;
                            image[i, j].Green = 0;
                            image[i, j].Blue = 0;
                        }
                        else
                        {
                            image[i, j].Red = 255;
                            image[i, j].Green = 255;
                            image[i, j].Blue = 255;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("La conversion de l'image en noir et blanc n'a pas pu se faire." + '\n' + e.Message);
            }


        }

        /// <summary>
        /// Méthode permettant de mettre l'image en nuances de gris
        /// </summary>
        public void GrayScale()
        {
            try
            {
                for (int i = 0; i < hauteur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        byte moyenne = (byte)((image[i, j].Red + image[i, j].Green + image[i, j].Blue) / 3); // (byte) devant car par défaut les opérateurs sont pour les int

                        image[i, j].Red = moyenne;
                        image[i, j].Blue = moyenne;
                        image[i, j].Green = moyenne;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("La conversion de l'image en nuances de gris n'a pas pu se faire." + '\n' + e.Message);
            }
        }

        /// <summary>
        /// Méthode d'agrandissement selon un coefficient
        /// </summary>
        /// <param name="coeff"></param>
        public void Bigger(int coeff)
        {
            Pixel[,] biggerImage = new Pixel[hauteur * coeff, largeur * coeff];

            hauteur = hauteur * coeff; // taille et hauteur sont modifiées en fontion du coeff
            largeur = largeur * coeff;
            tailleFichier = offset + (hauteur * largeur * 3); // idem taille fichier = h*l*3couleurs

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    biggerImage[i, j] = image[i / coeff, j / coeff];
                }
            }

            image = biggerImage;
        }

        /// <summary>
        /// Méthode de rétrécissement selon un coefficient
        /// </summary>
        /// <param name="coeff"></param>
        public void Smaller(int coeff)
        {
            Pixel[,] smallerImage = new Pixel[hauteur / coeff, largeur / coeff];

            hauteur = hauteur / coeff; // taille et hauteur sont modifiées en fontion du coeff
            largeur = largeur / coeff;
            tailleFichier = offset + (hauteur * largeur * 3); // idem taille fichier = h*l*3couleurs

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    smallerImage[i, j] = image[i * coeff, j * coeff];
                }
            }

            for (int i = 0; i < hauteur; i++) // s'assurer qu'il n'y a pas de pixel null
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (i < smallerImage.GetLength(0) && j < smallerImage.GetLength(1))
                    {
                        image[i, j] = smallerImage[i, j];
                    }
                    else
                    {
                        image[i, j] = new Pixel(0, 0, 0);
                    }
                }
            }

                image = smallerImage;
        }

        /// <summary>
        /// Méthode permettant de retourner l'image par l'horizontale : effet miroir
        /// </summary>
        public void Mirror()
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur / 2; j++)
                {
                    Pixel tamp = image[i, j];
                    image[i, j] = image[i, largeur - j - 1];
                    image[i, largeur - j - 1] = tamp;
                }
            }
        }

        /// <summary>
        /// Méthodes de rotation
        /// </summary>
        public void Rotation90()
        {
            Pixel[,] rotationImage = new Pixel[largeur, hauteur]; // inverse largeur et hauteur

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    rotationImage[j, i] = image[i, j];
                }
            }

            image = rotationImage;

            int tamp = hauteur; // utiliser tampon pour échanger valeurs hauteur & largeur
            hauteur = largeur;
            largeur = tamp;
        }

        public void Rotation180()
        {
            Pixel[,] rotationImage = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    rotationImage[i, j] = image[hauteur - 1 - i, largeur - 1 - j];
                }
            }

            image = rotationImage;
        }

        public void Rotation270()
        {
            Pixel[,] rotationImage = new Pixel[largeur, hauteur]; /// inverse largeur et hauteur

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    rotationImage[j, i] = image[hauteur - 1 - i, largeur - 1 - j];
                }
            }

            image = rotationImage;

            int tamp = hauteur;
            hauteur = largeur;
            largeur = tamp;
        }        

        /// <summary>
        /// Méthode de rotation tout angle : angle saisi par l'utilisateur
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(int angle)
        {
            Pixel[,] newImage = new Pixel[Math.Max(hauteur, largeur), Math.Max(hauteur, largeur)];

            double radAngle = angle * Math.PI / 180.0;
            double ligne;
            double colonne;            

            int[] centre = new int[2]; // calcul centre de rotation
            centre[0] = hauteur / 2;
            centre[1] = largeur / 2;


            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j <largeur; j++)
                {
                    colonne = (j - centre[1]) * Math.Cos(radAngle) - (i - centre[0]) * Math.Sin(radAngle); // matrice de rotation : [[cos, -sin],[sin, cos]]
                    ligne = (j - centre[1]) * Math.Sin(radAngle) + (i - centre[0]) * Math.Cos(radAngle);                   

                    if (ligne + centre[0] > 0 && ligne + centre[0] < hauteur && colonne + centre[1] > 0 && colonne + centre[1] < largeur)
                    {
                        newImage[i, j] = image[(int)ligne + centre[0], (int)colonne + centre[1]];
                    }

                }
            }

            for (int i = 0; i < hauteur ; i++) // remplir en noir les pixels null
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j] = newImage[i, j];

                    if (image[i, j] == null)
                    {
                        image[i, j] = new Pixel(0, 0, 0);
                    }
                }

            }
        }

        /// <summary>
        /// Méthode de convolution pour : détection de contours, renforcement bords, flou, repoussage
        /// </summary>
        /// <param name="noyau"></param>
        /// <param name="coeffAttenuation"></param>
        public void Convolution(int[,] noyau, double coeffAttenuation)
        {
            Pixel[,] newImage = new Pixel[hauteur, largeur];

            int sommeCoeffNoyau = 0;

            for (int i = 0; i < noyau.GetLength(0); i++)
            {
                for (int j = 0; j < noyau.GetLength(1); j++)
                {
                    sommeCoeffNoyau += noyau[i, j]; // calcul somme des coeff de la matrice de convo (noyau)
                }
            }

            if (sommeCoeffNoyau == 0) // pour ne pas diviser par 0
            {
                sommeCoeffNoyau = 1;
            }

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    double R = 0;
                    double G = 0;
                    double B = 0;

                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (i == 0 || i == image.GetLength(0) - 1 || j == 0 || j == image.GetLength(1) - 1) // pixels au bord de l'image non traités (éviter out of bounds durant convo)
                            {
                                newImage[i, j] = image[i, j];
                            }
                            else
                            {
                                R += (image[i + k, j + l].Red * noyau[k + 1, l + 1] * coeffAttenuation) / sommeCoeffNoyau;
                                G += (image[i + k, j + l].Green * noyau[k + 1, l + 1] * coeffAttenuation) / sommeCoeffNoyau;
                                B += (image[i + k, j + l].Blue * noyau[k + 1, l + 1] * coeffAttenuation) / sommeCoeffNoyau;
                            }
                        }
                    }

                    newImage[i, j] = new Pixel((byte)R, (byte)G, (byte)B);
                }
            }

            image = newImage;
        }        

        /// <summary>
        /// Méthode pour l'explosion de couleurs (création)
        /// </summary>
        /// <param name="noyau"></param>
        public void Creation(int[,] noyau)
        {
            Pixel[,] newImage = new Pixel[hauteur, largeur];

            int sommeCoeffNoyau = 0;

            for (int i = 0; i < noyau.GetLength(0); i++)
            {
                for (int j = 0; j < noyau.GetLength(1); j++)
                {
                    sommeCoeffNoyau += noyau[i, j]; // calcul somme des coeff de la matrice de convo (noyau)
                }
            }

            if (sommeCoeffNoyau == 0) // pour ne pas diviser par 0
            {
                sommeCoeffNoyau = 1;
            }

            for (int i = 0; i < hauteur; i++) // initialiser la newImage en mettant tous les pixels en noirs (éviter erreur out of bounds)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newImage[i, j] = new Pixel(0, 0, 0);
                }
            }

            for (int i = 1; i < hauteur - 2; i++)
            {
                for (int j = 1; j < largeur - 2; j++)
                {
                    for (int k = 0; k < noyau.GetLength(0); k++)
                    {
                        for (int l = 0; l < noyau.GetLength(1); l++) 
                        {
                            if (Math.Abs((Convert.ToInt32(image[i + k, j + l].Red) * noyau[k, l]) / sommeCoeffNoyau) > 255) // check que la valeur calculée n'est pas sup à 255...
                            {
                                newImage[i, j].Red += 255;
                            }
                            else
                            {
                                newImage[i, j].Red += Convert.ToByte(Math.Abs((Convert.ToInt32(image[i + k, j + l].Red) * noyau[k, l]) / sommeCoeffNoyau)); // ...pour utiliser Convert.ToByte
                            }
                            if (Math.Abs((Convert.ToInt32(image[i + k, j + l].Green) * noyau[k, l]) / sommeCoeffNoyau) > 255)
                            {
                                newImage[i, j].Green += 255;
                            }
                            else
                            {
                                newImage[i, j].Green += Convert.ToByte(Math.Abs((Convert.ToInt32(image[i + k, j + l].Green) * noyau[k, l]) / sommeCoeffNoyau));
                            }
                            if (Math.Abs((Convert.ToInt32(image[i + k, j + l].Blue) * noyau[k, l]) / sommeCoeffNoyau) > 255)
                            {
                                newImage[i, j].Blue += 255;
                            }
                            else
                            {
                                newImage[i, j].Blue += Convert.ToByte(Math.Abs((Convert.ToInt32(image[i + k, j + l].Blue) * noyau[k, l]) / sommeCoeffNoyau));
                            }
                        }
                    }
                }
            }

            image = newImage;
        }
        
        /// <summary>
        /// Méthode créant une fractale de Mandelbrot
        /// </summary>
        public static void Mandelbrot()
        {
            Pixel[,] newImage = new Pixel[400, 400];

            byte[] header = { 66, 77, 54, 83, 7, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 144, 1, 0, 0, 144, 1, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 83, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // taille image : 400x400 soit 0,83,7,0 en byte
            // taille fichier = taille image + 54 : 54,83,7,0 en byte
            // 400 en byte : 144,1,0,0

            for (int i = 0; i < 400; i++) // initialiser la newImage en mettant tous les pixels en noirs (éviter erreur out of bounds)
            {
                for (int j = 0; j < 400; j++)
                {
                    newImage[i, j] = new Pixel(0, 0, 0);
                }
            }

            for (int x = 0; x < newImage.GetLength(1); x++) // parcourt l'axe des réels
            {                                                                                             
                for (int y = 0; y < newImage.GetLength(0); y++) // parcourt l'axe des imaginaires 
                {
                    // origine du repère = centre image
                    // *0.01 pour zoom : multiplier pour augmenter précision (échelle + petite) sinon mini fractale
 
                    // c : coordonnée à laquelle un certain pixel sera colorié -> c = x+iy
                    Complex c = new Complex((y - newImage.GetLength(0)/2)*(float)0.01, (x - newImage.GetLength(1) / 2)*(float)0.01);                    
                    Complex z = new Complex(0, 0);

                    double k = 0;

                    while ((z.Module() < 2) && (k < 100)) // tant que le module de z est inférieur à 2 et l'itération inférieure à 100 (sinon itération infinie)
                    {
                        // application de la formule de recurrence : Z(n+1) = Z(n)^2 + c
                        z = Complex.Somme(Complex.Produit(z, z), c);

                        if ((255 * (1 - z.Module() / 2)) < 255 && (255 * (1 - z.Module() / 2)) >= 0)
                        {
                           // newImage[x, y] = new Pixel(Convert.ToByte(255 * (1 - k / 100)), 0, Convert.ToByte(255 * (1 - z.Module() / 2))); // rouge au léger dégradé    
                           newImage[x, y] = new Pixel(Convert.ToByte(Math.Abs(100 * c.Reel)), Convert.ToByte(Math.Abs(100 * c.Imaginaire)), 255); // cocktail
                        }  // 100*c.Reel/Im : dégradé de couleur

                        k++;
                    }

                    if (z.Module() >= 2) // Mandelbrot : stopper itération qd module < 2
                    {
                        newImage[x, y] = new Pixel(0, 0, 0); // le reste des pixels en noir
                    }

                }
            }

            byte[] fractale = new byte[54 + (400 * 400 * 3)]; // concaténer header et image

            for (int i = 0; i < 54; i++) // ajout header
            {
                fractale[i] = header[i];
            }

            int l = 54;

            for (int i = 0; i < 400; i++) // ajout image fractale créée ci-dessus, après le header
            {
                for (int j = 0; j < 400; j++)
                {
                    fractale[l] = newImage[i, j].Red;
                    fractale[l + 1] = newImage[i, j].Green;
                    fractale[l + 2] = newImage[i, j].Blue;

                    l += 3;
                }
            }

            File.WriteAllBytes("sortieFractaleMandelbrot.bmp", fractale);
        }


        public void Histogramme()
        {
            Pixel[,] newImage = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++) // initialiser la newImage en mettant tous les pixels en noirs (éviter erreur out of bounds)
            {
                for (int j = 0; j < largeur; j++)
                {
                    newImage[i, j] = new Pixel(0, 0, 0);
                }
            }

            double somme = 0;
            double percent = 0;

            // taux de rouge
            for (int i = 0; i < largeur; i++) 
            {                
                for(int j = 0; j < hauteur; j++)
                {
                    somme += image[j, i].Red;
                }

                // moyenne de rouge dans la colonne : moyenne = somme/hauteur
                // pourcentage sur 255 = moyenne*hauteur/255
                // --> percent = somme/255

                percent = somme / 255;
                int percentInt = Convert.ToInt32(Math.Round(percent));

                newImage[percentInt, i] = new Pixel(255, 0, 0); // colorer en rouge le point situé à la hauteur "percent" de la colonne

                somme = 0;
            }

            // taux de vert
            for (int i = 0; i < largeur; i ++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    somme += image[j, i].Green;
                }

                percent = somme / 255;
                int percentInt = Convert.ToInt32(Math.Round(percent));

                newImage[percentInt, i] = new Pixel(0, 255, 0); // colorer en vert le point situé à la hauteur "percent" de la colonne

                somme = 0;
                
            }

            // taux de bleu
            for (int i = 0; i < largeur; i ++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    somme += image[j, i].Blue;
                }

                percent = somme / 255;
                int percentInt = Convert.ToInt32(Math.Round(percent));

                newImage[percentInt, i] = new Pixel(0, 0, 255); // colorer en bleu le point situé à la hauteur "percent" de la colonne

                somme = 0;
            }

            image = newImage;
        } 
        
        /// <summary>
        /// Méthode permettant de coder une image dans une image, les images sont superposées mais n'est visible qu'une seule des 2
        /// </summary>
        /// <param name="imageACacher"></param>
        public void CodageImage(MyImage imageACacher)
        {
            Pixel[,] newImage = new Pixel[hauteur, largeur];            

            if (imageACacher.Hauteur > hauteur || imageACacher.Largeur > largeur) // on ne peut pas cacher une image dans une image plus petite
            {
                Console.WriteLine("Impossible : L'image à cacher est trop grande pour être cachée dans l'image initiale.");
            }
            else
            {
                for (int i = 0; i < hauteur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        newImage[i, j] = image[i, j];
                    }
                }

                for (int i = 0; i < imageACacher.Hauteur; i++)
                {
                    for (int j = 0; j < imageACacher.Largeur; j++)
                    {
                        newImage[i, j].Red = (byte)(image[i, j].Red - (image[i, j].Red % 16) + (imageACacher.image[i, j].Red - (imageACacher.image[i, j].Red % 16))/16);
                        newImage[i, j].Green = (byte)(image[i, j].Green - (image[i, j].Green % 16) + (imageACacher.image[i, j].Green - (imageACacher.image[i, j].Green % 16))/16);
                        newImage[i, j].Blue = (byte)(image[i, j].Blue - (image[i, j].Blue % 16) + (imageACacher.image[i, j].Blue - (imageACacher.image[i, j].Blue % 16))/16);

                        // binaire : (fort) 2^7 + 2^6 + ... + 2^1 + 2^0 (faible)
                        // 2^4 = 16 car je remplace 4 bits sur les 8 bits de l'octet 
                        // %16 est le reste de la division par 2^4 afin d'obtenir les bits de poids faible
                        // (bits poids fort - bits poids faible de l'image cachante) + (bits poids fort - bits poids faible image cachée)/2^4
                        // diviser par 16 les bits de poids fort de l'image cachée pour normaliser afin qu'ils deviennent les bits de poids faible de la nouvelle image
                    }
                }

                image = newImage;
            }            
        }

        /// <summary>
        /// Méthode permettant de décoder une image cachée dans une autre
        /// </summary>
        public void DecodageImage()
        {
            Pixel[,] imageDecodee = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    imageDecodee[i, j] = new Pixel((byte)((image[i, j].Red % 16) * 16), // extraire les bits de poids faible et les transformer en bits de poids fort
                                                   (byte)((image[i, j].Green % 16) * 16), 
                                                   (byte)((image[i, j].Blue % 16) * 16));
                }
            }

            image = imageDecodee;
        }
    }
}

