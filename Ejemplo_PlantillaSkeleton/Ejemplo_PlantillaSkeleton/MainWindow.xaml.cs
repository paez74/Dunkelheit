using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/* -- Bibliotecas añadidas --*/
using Microsoft.Kinect;
using System.IO;
using System.Windows.Threading;
using System.Media;
/*---------------------------*/

namespace Ejemplo_PlantillaSkeleton
{
    /// <summary>
    /// Capítulo: Reflejar el movimiento con imágenes
    /// Ejemplo: Obtener la posición de la mano derecha (De cualquier persona, no se selecciona cual)
    /// Descripción: 
    ///              Este sencillo ejemplo muestra una ventana con un círculo del cual, su movimiento, refleja el 
    ///              movimiento de la mano derecha. Conforme se mueve la mano se mueve el círculo.
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor miKinect;  //Representa el Kinect conectado

        /* ----------------------- Área para las variables ------------------------- */
        double dMano_X;            //Representa la coordenada X de la mano derecha
        double dMano_Y;            //Representa la coordenada Y de la mano derecha
        Point joint_Point = new Point(); //Permite obtener los datos del Joint
                                         /* ------------------------------------------------------------------------- */
                                         //Variables	que	se	emplearán	para	almacenar	el	centro	del	aro
        int iKills = 0;


        
         


        DispatcherTimer timer;

        //	Estructura	para	almacenar	la	información	del	objeto
        struct Monstruos
        {
            public double dPosX;
            public double dPosY;
            public double dAncho;
            public double dAlto;
            public int tipo; // 1-blanco , 2-rojo , 3- amarillo; 
            public bool activo;
            public int iSize;

        }

        //	Estructura	para	almacenar	la	información	del	objeto
        struct Fuego
        {
            public double dPosX;
            public double dPosY;
            public double dAncho;
            public double dAlto;
            public bool colisionando; // con el pointer 
            public int tipo;  // 1-rojo , 2-azul , 3-dorado 

        }
        //Objetos	que	emplean	la	aplicación
        Fuego obRedFire1, obRedFire2, obBlueFire1, obBlueFire2, obGoldFire1, obGoldFire2,obPuntero;
        Monstruos  obAfraidRed1, obAfraidWhite1,obAfraidYellow1, obAfraidRed2, obAfraidWhite2, obAfraidYellow2;
        Random rnd = new Random();
        int monster = 1;
        SoundPlayer monsterScream; 
    
        public MainWindow()
        {
            InitializeComponent();

            
            monsterScream = new SoundPlayer(@"C:\Users\eduar_000\Desktop\Apps\Dunkelheit\Sounds\Monster.wav");

          
    
            // Enfocar el Canvas
            MainCanvas.Focusable = true;
            MainCanvas.Focus();

            // creo el dispatcher time 
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 2, 0);

            // crear el evento 
            timer.Tick += new EventHandler(Growth_Tick);
            // Iniciar el evento 
            timer.IsEnabled = true;

            InitializeMonstersFire(); // Inicializar componentes 
            

            
            // Realizar configuraciones e iniciar el Kinect

            Kinect_Config();
        }

        private void Growth_Tick(object sender, EventArgs e)
        {
            

           

            UpdateMonsters();
            if (checarColisionMonstruo(obRedFire1, obAfraidRed1))
            {
                Afraid.Height = 70;
                Afraid.Width = 70;
                obAfraidRed1.dAlto = 70;
                obAfraidRed1.dAncho = 70;
                Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidRed1.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                obAfraidRed1.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
                
                obAfraidRed1.activo = false;
                Afraid.Visibility = Visibility.Hidden;

                RedFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                RedFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obRedFire1.dPosX = (double)RedFire1.GetValue(Canvas.LeftProperty);
                obRedFire1.dPosY = (double)RedFire1.GetValue(Canvas.TopProperty);
                

            }
            else if (checarColisionMonstruo(obRedFire2, obAfraidRed1))
            {
                
                Afraid.Height = 70;
                Afraid.Width = 70;
                obAfraidRed1.dAlto = 70;
                obAfraidRed1.dAncho = 70;
                Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidRed1.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                obAfraidRed1.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);

                obAfraidRed1.activo = false;
                Afraid.Visibility = Visibility.Hidden;

                RedFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                RedFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obRedFire2.dPosX = (double)RedFire2.GetValue(Canvas.LeftProperty);
                obRedFire2.dPosY = (double)RedFire2.GetValue(Canvas.TopProperty);
                

            }
            else if (checarColisionMonstruo(obBlueFire1, obAfraidWhite1))
            {
                Afraidwhite.Height = 70;
                Afraidwhite.Width = 70;
                obAfraidWhite1.dAlto = 70;
                obAfraidWhite1.dAncho = 70;

                Afraidwhite.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidwhite.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidWhite1.activo = false;
                Afraidwhite.Visibility = Visibility.Hidden;

                BlueFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                BlueFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obBlueFire1.dPosX = (double)BlueFire1.GetValue(Canvas.LeftProperty);
                obBlueFire1.dPosY = (double)BlueFire1.GetValue(Canvas.TopProperty);
                
            }
            else if (checarColisionMonstruo(obBlueFire2, obAfraidWhite1))
            {
                Afraidwhite.Height = 70;
                Afraidwhite.Width = 70;
                obAfraidWhite1.dAlto = 70;
                obAfraidWhite1.dAncho = 70;

                Afraidwhite.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidwhite.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidWhite1.activo = false;
                Afraidwhite.Visibility = Visibility.Hidden;

                BlueFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                BlueFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obBlueFire2.dPosX = (double)BlueFire2.GetValue(Canvas.LeftProperty);
                obBlueFire2.dPosY = (double)BlueFire2.GetValue(Canvas.TopProperty);
                
            }
            else if (checarColisionMonstruo(obBlueFire1, obAfraidWhite2))
            {
                Afraidwhite2.Height = 70;
                Afraidwhite2.Width = 70;
                obAfraidWhite2.dAlto = 70;
                obAfraidWhite2.dAncho = 70;

                Afraidwhite2.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidwhite2.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidWhite2.activo = false;
                Afraidwhite2.Visibility = Visibility.Hidden;

                BlueFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                BlueFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obBlueFire1.dPosX = (double)BlueFire1.GetValue(Canvas.LeftProperty);
                obBlueFire1.dPosY = (double)BlueFire1.GetValue(Canvas.TopProperty);
                
            }
            else if (checarColisionMonstruo(obBlueFire2, obAfraidWhite2))
            {
                Afraidwhite2.Height = 70;
                Afraidwhite2.Width = 70;
                obAfraidWhite2.dAlto = 70;
                obAfraidWhite2.dAncho = 70;

                Afraidwhite2.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidwhite2.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidWhite2.activo = false;
                Afraidwhite2.Visibility = Visibility.Hidden;

                BlueFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                BlueFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obBlueFire2.dPosX = (double)BlueFire2.GetValue(Canvas.LeftProperty);
                obBlueFire2.dPosY = (double)BlueFire2.GetValue(Canvas.TopProperty);
                
            }
            else if (checarColisionMonstruo(obGoldFire1,obAfraidYellow1))
            {
                Afraidyellow.Height = 70;
                Afraidyellow.Width = 70;
                obAfraidYellow1.dAlto = 70;
                obAfraidYellow1.dAncho = 70;

                Afraidyellow.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidyellow.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidYellow1.activo = false;
                Afraidyellow.Visibility = Visibility.Hidden;

                GoldFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                GoldFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obGoldFire1.dPosX = (double)GoldFire1.GetValue(Canvas.LeftProperty);
                obGoldFire1.dPosY = (double)GoldFire1.GetValue(Canvas.TopProperty);
                

            }
            else if (checarColisionMonstruo(obGoldFire2, obAfraidYellow1))
            {
                Afraidyellow.Height = 70;
                Afraidyellow.Width = 70;
                obAfraidYellow1.dAlto = 70;
                obAfraidYellow1.dAncho = 70;

                Afraidyellow.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                Afraidyellow.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                obAfraidYellow1.activo = false;
                Afraidyellow.Visibility = Visibility.Hidden;

                GoldFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));

                GoldFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
                obGoldFire2.dPosX = (double)GoldFire2.GetValue(Canvas.LeftProperty);
                obGoldFire2.dPosY = (double)GoldFire2.GetValue(Canvas.TopProperty);
                
            }

            switch (monster)
            {
                case 1:
                    if (obAfraidRed1.activo != true)
                    {
                        obAfraidRed1.activo = true;
                        Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        obAfraidRed1.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                        obAfraidRed1.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
                        Afraid.Visibility = Visibility.Visible;

                    }
                    monster++;
                    break;
                case 2:
                    if (obAfraidRed2.activo != true)
                    {
                        obAfraidRed2.activo = true;
                        //Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        //Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        obAfraidRed2.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                        obAfraidRed2.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
                        //Afraid.Visibility = Visibility.Visible;

                    }
                    monster++;
                    break;
                case 3:
                    if (obAfraidWhite1.activo != true)
                    {
                        obAfraidWhite1.activo = true;
                        Afraidwhite.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        Afraidwhite.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        obAfraidWhite1.dPosX = (double)Afraidwhite.GetValue(Canvas.LeftProperty);
                        obAfraidWhite1.dPosY = (double)Afraidwhite.GetValue(Canvas.TopProperty);
                        Afraidwhite.Visibility = Visibility.Visible;

                    }
                    monster++;
                    break;
                case 4:
                    if (obAfraidWhite2.activo != true)
                    {
                        obAfraidWhite2.activo = true;
                        Afraidwhite2.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        Afraidwhite2.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        obAfraidWhite2.dPosX = (double)Afraidwhite2.GetValue(Canvas.LeftProperty);
                        obAfraidWhite2.dPosY = (double)Afraidwhite2.GetValue(Canvas.TopProperty);
                        Afraidwhite2.Visibility = Visibility.Visible;
                    }
                    monster++;
                    break;
                case 5:
                    if (obAfraidYellow1.activo != true)
                    {
                        obAfraidYellow1.activo = true;
                        Afraidyellow.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        Afraidyellow.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        obAfraidYellow1.dPosX = (double)Afraidyellow.GetValue(Canvas.LeftProperty);
                        obAfraidYellow1.dPosY = (double)Afraidyellow.GetValue(Canvas.TopProperty);
                        Afraidyellow.Visibility = Visibility.Visible;

                    }
                    monster = 1;
                    break;
                

            }
        }
        
       
        /* -- Área para el método que utiliza los datos proporcionados por Kinect -- */
        /// <summary>
        /// Método que realiza las manipulaciones necesarias sobre el Skeleton trazado
        /// </summary>
        private void usarSkeleton(Skeleton skeleton)
        {
            Joint joint1 = skeleton.Joints[JointType.HandRight];



            // Si el Joint está listo obtener las coordenadas
            if (joint1.TrackingState == JointTrackingState.Tracked)
            {
                // Obtener coordenadas
                joint_Point = this.SkeletonPointToScreen(joint1.Position);
                dMano_X = joint_Point.X;
                dMano_Y = joint_Point.Y;

                
                // Modificar coordenadas del indicador que refleja el movimiento (Ellipse rojo)
                Puntero.SetValue(Canvas.TopProperty, dMano_Y - 12.5);
                Puntero.SetValue(Canvas.LeftProperty, dMano_X - 12.5);

                score.Content = iKills.ToString();

                ////Puntero
                obPuntero.dPosX = (double)Puntero.GetValue(Canvas.LeftProperty);
                obPuntero.dPosY = (double)Puntero.GetValue(Canvas.TopProperty);
                obPuntero.dAncho = Puntero.Width;
                obPuntero.dAlto = Puntero.Height;
                // Indicar Id de la persona que es trazada
                LID.Content = skeleton.TrackingId;

                if (checarColisionFuego(obPuntero, obRedFire1) && obRedFire1.colisionando == false)
                {
                    AllFalse();
                    obRedFire1.colisionando = true;
                    obRedFire1.dPosX = (double)RedFire1.GetValue(Canvas.LeftProperty);
                    obRedFire1.dPosY = (double)RedFire1.GetValue(Canvas.TopProperty);

                }
                else if (checarColisionFuego(obPuntero, obRedFire2) && obRedFire2.colisionando == false)
                {
                    AllFalse();
                    obRedFire2.colisionando = true;
                    obRedFire2.dPosX = (double)RedFire2.GetValue(Canvas.LeftProperty);
                    obRedFire2.dPosY = (double)RedFire2.GetValue(Canvas.TopProperty);
                }
                else if (checarColisionFuego(obPuntero, obBlueFire1) && obBlueFire1.colisionando == false)
                {
                    AllFalse();
                    obBlueFire1.colisionando = true;
                    obBlueFire1.dPosX = (double)BlueFire1.GetValue(Canvas.LeftProperty);
                    obBlueFire1.dPosY = (double)BlueFire1.GetValue(Canvas.TopProperty);

                }
                else if (checarColisionFuego(obPuntero, obBlueFire2) && obBlueFire2.colisionando == false)
                {
                    AllFalse();
                    obBlueFire2.colisionando = true;
                    obBlueFire2.dPosX = (double)BlueFire2.GetValue(Canvas.LeftProperty);
                    obBlueFire2.dPosY = (double)BlueFire2.GetValue(Canvas.TopProperty);
                }
                else if (checarColisionFuego(obPuntero, obGoldFire1) && obGoldFire1.colisionando == false)
                {
                    AllFalse();
                    obGoldFire1.colisionando = true;
                    obGoldFire1.dPosX = (double)GoldFire1.GetValue(Canvas.LeftProperty);
                    obGoldFire1.dPosY = (double)GoldFire1.GetValue(Canvas.TopProperty);

                }
                else if (checarColisionFuego(obPuntero, obGoldFire2) && obGoldFire2.colisionando == false)
                {
                    AllFalse();
                    obGoldFire2.colisionando = true;
                    obGoldFire2.dPosX = (double)GoldFire2.GetValue(Canvas.LeftProperty);
                    obGoldFire2.dPosY = (double)GoldFire2.GetValue(Canvas.TopProperty);
                }


                if (obRedFire1.colisionando == true)
                {
                    AllFalse();
                    obRedFire1.colisionando = true;
                    RedFire1.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    RedFire1.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obRedFire1.dPosX = (double)RedFire1.GetValue(Canvas.LeftProperty);
                    obRedFire1.dPosY = (double)RedFire1.GetValue(Canvas.TopProperty);
                }
                else if (obRedFire2.colisionando == true)
                {
                    AllFalse();
                    obRedFire2.colisionando = true;
                    RedFire2.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    RedFire2.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obRedFire2.dPosX = (double)RedFire2.GetValue(Canvas.LeftProperty);
                    obRedFire2.dPosY = (double)RedFire2.GetValue(Canvas.TopProperty);
                }
                else if (obBlueFire1.colisionando == true)
                {
                    AllFalse();
                    obBlueFire1.colisionando = true;
                    BlueFire1.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    BlueFire1.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obBlueFire1.dPosX = (double)BlueFire1.GetValue(Canvas.LeftProperty);
                    obBlueFire1.dPosY = (double)BlueFire1.GetValue(Canvas.TopProperty);
                }
                else if (obBlueFire2.colisionando == true)
                {
                    AllFalse();
                    obBlueFire2.colisionando = true;
                    BlueFire2.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    BlueFire2.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obBlueFire2.dPosX = (double)BlueFire2.GetValue(Canvas.LeftProperty);
                    obBlueFire2.dPosY = (double)BlueFire2.GetValue(Canvas.TopProperty);
                }
                else if (obGoldFire1.colisionando == true)
                {
                    AllFalse();
                    obGoldFire1.colisionando = true;
                    GoldFire1.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    GoldFire1.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obGoldFire1.dPosX = (double)GoldFire1.GetValue(Canvas.LeftProperty);
                    obGoldFire1.dPosY = (double)GoldFire1.GetValue(Canvas.TopProperty);
                }
                else if (obGoldFire2.colisionando == true)
                {
                    AllFalse();
                    obGoldFire2.colisionando = true;
                    GoldFire2.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    GoldFire2.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obGoldFire2.dPosX = (double)GoldFire2.GetValue(Canvas.LeftProperty);
                    obGoldFire2.dPosY = (double)GoldFire2.GetValue(Canvas.TopProperty);
                }
            }

}

        private bool checarColisionFuego(Fuego puntero, Fuego fire)
        {
            if (puntero.dPosX + puntero.dAncho < fire.dPosX)     //Colisión	por	la	izquierda	de	ob2
                return false;
            if (puntero.dPosY + puntero.dAlto < fire.dPosY)      //Colisión	por	arriba	de	ob2
                return false;
            if (puntero.dPosY > fire.dPosY + fire.dAlto)      //Colisión	por	abajo	ob2
                return false;
            if (puntero.dPosX > fire.dPosX + fire.dAncho) //Colisión	por	la	derecha	ob2
                return false;
            return true;
        }

        private bool checarColisionMonstruo(Fuego fuego, Monstruos monstruo)
        {
            if (monstruo.dPosX + monstruo.dAncho < fuego.dPosX)     //Colisión	por	la	izquierda	de	ob2
                return false;
            if (monstruo.dPosY + monstruo.dAlto < fuego.dPosY)      //Colisión	por	arriba	de	ob2
                return false;
            if (monstruo.dPosY > fuego.dPosY + fuego.dAlto)      //Colisión	por	abajo	ob2
                return false;
            if (monstruo.dPosX > fuego.dPosX + fuego.dAncho) //Colisión	por	la	derecha	ob2
                return false;

            monsterScream.Play();
            iKills++;

            return true;
        }


        /*Checar Sizeee*/
        private void checarsize(Monstruos monstruo)
        {

            if (monstruo.iSize>11)
            {
                gameover.Visibility = Visibility.Visible;
            }
        }




        private void InitializeMonstersFire()
        {
            Random rnd = new Random();
            gameover.Visibility = Visibility.Hidden;

            //Inicio las imagenes
            obAfraidRed1.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
            obAfraidRed1.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
            obAfraidRed1.dAlto = Afraid.Height;
            obAfraidRed1.dAncho = Afraid.Width;
            obAfraidRed1.tipo = 1;
            obAfraidRed1.activo = false;
            Afraid.Visibility = Visibility.Collapsed;
            obAfraidRed1.iSize = 0;
            

            obAfraidWhite1.dPosX = (double)Afraidwhite.GetValue(Canvas.LeftProperty);
            obAfraidWhite1.dPosY = (double)Afraidwhite.GetValue(Canvas.TopProperty);
            obAfraidWhite1.dAlto = Afraidwhite.Height;
            obAfraidWhite1.dAncho = Afraidwhite.Width;
            obAfraidWhite1.tipo = 2;
            obAfraidWhite1.activo = false;
            Afraidwhite.Visibility = Visibility.Collapsed;
            obAfraidWhite1.iSize = 0;


            obAfraidWhite2.dPosX = (double)Afraidwhite2.GetValue(Canvas.LeftProperty);
            obAfraidWhite2.dPosY = (double)Afraidwhite2.GetValue(Canvas.TopProperty);
            obAfraidWhite2.dAlto = Afraidwhite2.Height;
            obAfraidWhite2.dAncho = Afraidwhite2.Width;
            obAfraidWhite2.tipo = 2;
            obAfraidWhite2.activo = false;
            Afraidwhite2.Visibility = Visibility.Collapsed;
            obAfraidWhite2.iSize = 0;

            obAfraidYellow1.dPosX = (double)Afraidyellow.GetValue(Canvas.LeftProperty);
            obAfraidYellow1.dPosY = (double)Afraidyellow.GetValue(Canvas.TopProperty);
            obAfraidYellow1.dAlto = Afraidyellow.Height;
            obAfraidYellow1.dAncho = Afraidyellow.Width;
            obAfraidYellow1.tipo = 3;
            obAfraidYellow1.activo = false;
            Afraidyellow.Visibility = Visibility.Collapsed;
            obAfraidWhite1.iSize = 0;

            //// inicio en random la location del Fuego izquierdos
            ///Redfire1
            RedFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));
            RedFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395-73));
            obRedFire1.dPosX = (double)RedFire1.GetValue(Canvas.LeftProperty);
            obRedFire1.dPosY = (double)RedFire1.GetValue(Canvas.TopProperty);
            obRedFire1.dAlto = RedFire1.Height;
            obRedFire1.dAncho = RedFire1.Width;
            obRedFire1.tipo = 1;
            obRedFire1.colisionando = false;
            ///Goldfire1
            BlueFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));
            BlueFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395 - 73));
            obBlueFire1.dPosX = (double)BlueFire1.GetValue(Canvas.LeftProperty);
            obBlueFire1.dPosY = (double)BlueFire1.GetValue(Canvas.TopProperty);
            obBlueFire1.dAlto = BlueFire1.Height;
            obBlueFire1.dAncho = BlueFire1.Width;
            obBlueFire1.tipo = 2;
            obBlueFire1.colisionando = false;

            ///Yellowfire1
            GoldFire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));
            GoldFire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395 - 73));
            obGoldFire1.dPosX = (double)GoldFire1.GetValue(Canvas.LeftProperty);
            obGoldFire1.dPosY = (double)GoldFire1.GetValue(Canvas.TopProperty);
            obGoldFire1.dAlto = GoldFire1.Height;
            obGoldFire1.dAncho = GoldFire1.Width;
            obGoldFire1.tipo = 3;
            obGoldFire1.colisionando = false;


            //// inicio en random la location del Fuego derechos
            RedFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(531, 619-70));
            RedFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395-73));
            obRedFire2.dPosX = (double)RedFire2.GetValue(Canvas.LeftProperty);
            obRedFire2.dPosY = (double)RedFire2.GetValue(Canvas.TopProperty);
            obRedFire2.dAlto = RedFire2.Height;
            obRedFire2.dAncho = RedFire2.Width;
            obRedFire2.tipo = 1;
            obRedFire2.colisionando = false;

            BlueFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(531, 619 - 70));
            BlueFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395 - 73));
            obBlueFire2.dPosX = (double)BlueFire2.GetValue(Canvas.LeftProperty);
            obBlueFire2.dPosY = (double)BlueFire2.GetValue(Canvas.TopProperty);
            obBlueFire2.dAlto = BlueFire2.Height;
            obBlueFire2.dAncho = BlueFire2.Width;
            obBlueFire2.tipo = 2;
            obBlueFire2.colisionando = false;

            GoldFire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(531, 619 - 70));
            GoldFire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 395 - 73));
            obGoldFire2.dPosX = (double)GoldFire2.GetValue(Canvas.LeftProperty);
            obGoldFire2.dPosY = (double)GoldFire2.GetValue(Canvas.TopProperty);
            obGoldFire2.dAlto = GoldFire2.Height;
            obGoldFire2.dAncho = GoldFire2.Width;
            obGoldFire2.tipo = 3;
            obGoldFire2.colisionando = false;

        }
        private void AllFalse()
        {
            obRedFire1.colisionando = false;
            obRedFire2.colisionando = false;
            obBlueFire1.colisionando = false;
            obBlueFire2.colisionando = false;
            obGoldFire1.colisionando = false;
            obGoldFire2.colisionando = false;
            
        }
        private void UpdateMonsters()
        {
            
            //Afraid.SetValue(HeightProperty, obAfraidRed1.dAlto);

            if (obAfraidRed1.activo)
            {

                Afraid.Height *= 1.1;
                Afraid.Width *= 1.1;
                obAfraidRed1.dAlto = Afraid.Height;
                obAfraidRed1.dAncho = Afraid.Width;
                obAfraidRed1.dPosX = (double)Afraid.GetValue(LeftProperty);
                obAfraidRed1.dPosY = (double)Afraid.GetValue(TopProperty);
                obAfraidRed1.iSize++;
                checarsize(obAfraidRed1);
            }
            //Afraidwhite.SetValue(Canvas.LeftProperty, obAfraidWhite1.dPosX);
            //Afraidwhite.SetValue(Canvas.TopProperty, obAfraidWhite1.dPosY);

            if (obAfraidWhite1.activo)
            { 
            Afraidwhite.Height *= 1.1;
            Afraidwhite.Width *= 1.1;
               
                obAfraidWhite1.dAlto = Afraidwhite.Height;
                obAfraidWhite1.dAncho = Afraidwhite.Width;
                obAfraidWhite1.dPosX = (double)Afraidwhite.GetValue(LeftProperty);
                obAfraidWhite1.dPosY = (double)Afraidwhite.GetValue(TopProperty);
                obAfraidWhite1.iSize++;
                checarsize(obAfraidWhite1);
            }

            if (obAfraidWhite2.activo)
            {
                Afraidwhite2.Height *= 1.1;
                Afraidwhite2.Width *= 1.1;
                obAfraidWhite2.dAlto = Afraidwhite2.Height;
                obAfraidWhite2.dAncho = Afraidwhite2.Width;
                obAfraidWhite2.dPosX = (double)Afraidwhite2.GetValue(LeftProperty);
                obAfraidWhite2.dPosY = (double)Afraidwhite2.GetValue(TopProperty);
                obAfraidWhite2.iSize++;
                checarsize(obAfraidWhite2);
            }
            

            if (obAfraidYellow1.activo)
            {
                Afraidyellow.Height *= 1.1;
                Afraidyellow.Width *= 1.1;
                obAfraidYellow1.dAlto = Afraidyellow.Height;
                obAfraidYellow1.dAncho = Afraidyellow.Width;
                obAfraidYellow1.dPosX = (double)Afraidyellow.GetValue(LeftProperty);
                obAfraidYellow1.dPosY = (double)Afraidyellow.GetValue(TopProperty);
                obAfraidYellow1.iSize++;
                checarsize(obAfraidYellow1);
            }
            

            
           


        }
        /* ------------------------------------------------------------------------- */

        /* --------------------------- Métodos Nuevos ------------------------------ */

        /// <summary>
        /// Metodo que convierte un "SkeletonPoint" a "DepthSpace", esto nos permite poder representar las coordenadas de los Joints
        /// en nuestra ventana en las dimensiones deseadas.
        /// </summary>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convertertir un punto a "Depth Space" en una resolución de 640x480
            DepthImagePoint depthPoint = this.miKinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        /* ------------------------------------------------------------------------- */

        /// <summary>
        /// Método que realiza las configuraciones necesarias en el Kinect 
        /// así también inicia el Kinect para el envío de datos
        /// </summary>
        private void Kinect_Config()
        {
            // Buscamos el Kinect conectado con la propiedad KinectSensors, al descubrir el primero con el estado Connected
            // se asigna a la variable miKinect que lo representará (KinectSensor miKinect)
            miKinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            if (this.miKinect != null && !this.miKinect.IsRunning)
            {

                /* ------------------- Configuración del Kinect ------------------- */
                // Habilitar el SkeletonStream para permitir el trazo de "Skeleton"
                this.miKinect.SkeletonStream.Enable();

                // Enlistar al evento que se ejecuta cada vez que el Kinect tiene datos listos
                this.miKinect.SkeletonFrameReady += this.Kinect_FrameReady;
                /* ---------------------------------------------------------------- */

                // Enlistar el método que se llama cada vez que hay un cambio en el estado del Kinect
                KinectSensor.KinectSensors.StatusChanged += Kinect_StatusChanged;

                // Iniciar el Kinect
                try
                {
                    this.miKinect.Start();
                }
                catch (IOException)
                {
                    this.miKinect = null;
                }
                LEstatus.Content = "Conectado";
            }
            else
            {
                // Enlistar el método que se llama cada vez que hay un cambio en el estado del Kinect
                KinectSensor.KinectSensors.StatusChanged += Kinect_StatusChanged;
            }
        }
        /// <summary>
        /// Método que adquiere los datos que envia el Kinect, su contenido varía según la tecnología 
        /// que se esté utilizando (Cámara, SkeletonTraking, DepthSensor, etc)
        /// </summary>
        private void Kinect_FrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Arreglo que recibe los datos  
            Skeleton[] skeletons = new Skeleton[0];
            Skeleton skeleton;

            // Abrir el frame recibido y copiarlo al arreglo skeletons
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            // Seleccionar el primer Skeleton trazado
            skeleton = (from trackSkeleton in skeletons where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked select trackSkeleton).FirstOrDefault();

            if (skeleton == null)
            {
                LID.Content = "0";
                return;
            }
            LID.Content = skeleton.TrackingId;

            // Enviar el Skelton a usar
            this.usarSkeleton(skeleton);
        }
        /// <summary>
        /// Método que configura del Kinect de acuerdo a su estado(conectado, desconectado, etc),
        /// su contenido varia según la tecnología que se esté utilizando (Cámara, SkeletonTraking, DepthSensor, etc)
        /// </summary>
        private void Kinect_StatusChanged(object sender, StatusChangedEventArgs e)
        {

            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.miKinect == null)
                    {
                        this.miKinect = e.Sensor;
                    }

                    if (this.miKinect != null && !this.miKinect.IsRunning)
                    {
                        /* ------------------- Configuración del Kinect ------------------- */
                        // Habilitar el SkeletonStream para permitir el trazo de "Skeleton"
                        this.miKinect.SkeletonStream.Enable();

                        // Enlistar al evento que se ejecuta cada vez que el Kinect tiene datos listos
                        this.miKinect.SkeletonFrameReady += this.Kinect_FrameReady;
                        /* ---------------------------------------------------------------- */

                        // Iniciar el Kinect
                        try
                        {
                            this.miKinect.Start();
                        }
                        catch (IOException)
                        {
                            this.miKinect = null;
                        }
                        LEstatus.Content = "Conectado";
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (this.miKinect == e.Sensor)
                    {
                        /* ------------------- Configuración del Kinect ------------------- */
                        this.miKinect.SkeletonFrameReady -= this.Kinect_FrameReady;
                        /* ---------------------------------------------------------------- */

                        this.miKinect.Stop();
                        this.miKinect = null;
                        LEstatus.Content = "Desconectado";

                    }
                    break;
            }
        }
        /// <summary>
        /// Método que libera los recursos del Kinect cuando se termina la aplicación
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.miKinect != null && this.miKinect.IsRunning)
            {
                /* ------------------- Configuración del Kinect ------------------- */
                this.miKinect.SkeletonFrameReady -= this.Kinect_FrameReady;
                /* ---------------------------------------------------------------- */

                this.miKinect.Stop();
            }
        }
    }
}
