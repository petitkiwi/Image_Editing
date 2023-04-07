using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD2A5
{
    class Complex
    {
        /// attributs
        float reel;
        float imaginaire;

        // constructeur
        public Complex(float re, float im)
        {
            this.reel = re;
            this.imaginaire = im;
        }

        //propriétés
        public float Reel
        {
            get { return reel; }
            set { reel = value; }
        }
        public float Imaginaire
        {
            get { return imaginaire; }
            set { imaginaire = value; }
        }

        // méthodes

        public double Module() // module d'un complexe
        {
            return Math.Pow((Math.Pow(reel, 2) + Math.Pow(imaginaire, 2)), 0.5);
        }

        public static Complex Somme(Complex z1, Complex z2) // somme de 2 complexes
        {
            Complex somme = new Complex(z1.reel + z2.reel, z1.imaginaire + z2.imaginaire);
            return somme;
        }

        public static Complex Produit(Complex z1, Complex z2) // produit de 2 complexes
        {
            Complex prod = new Complex(z1.reel * z2.reel - z1.imaginaire * z2.imaginaire, z1.reel * z2.imaginaire + z2.reel*z1.imaginaire);
            return prod;
        }
    }
}
