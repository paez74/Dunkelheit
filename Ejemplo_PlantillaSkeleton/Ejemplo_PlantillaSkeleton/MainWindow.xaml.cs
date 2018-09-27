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
        double dXC, dYC;
        //Variables	que	almacenan	el	radio	de	cada	uno	de	los	círculos.
        double dRadioC1, dRadioC2;


        double iAnchoCanvas, iAltoCanvas;

        double dAncho = 742 - 70;	//Ancho	del	Canvas	– Ancho del fuego.
        double dAlto = 684 - 70;   // alto del canvas - alto del fuego 

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

        public MainWindow()
        {
            InitializeComponent();

            // Enfocar el Canvas
            MainCanvas.Focusable = true;
            MainCanvas.Focus();

            // creo el dispatcher time 
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);

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
                        //obAfraidWhite2.activo = true;
                        ////Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                        ////Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                        //obAfraidWhite2.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                        //obAfraidWhite2.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
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
                    monster++;
                    break;
                case 6:
                    if (obAfraidYellow2.activo != true)
                    {
                    //    obAfraidYellow2.activo = true;
                    //    //Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
                    //    //Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
                    //    obAfraidYellow2.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
                    //    obAfraidYellow2.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
                    }
                    monster = rnd.Next(1, 5);
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

                if (obRedFire1.colisionando == true)
                {
                    Fire1.SetValue(Canvas.TopProperty, Puntero.GetValue(Canvas.TopProperty));
                    Fire1.SetValue(Canvas.LeftProperty, Puntero.GetValue(Canvas.LeftProperty));
                    obRedFire1.dPosX = (double)Fire1.GetValue(Canvas.LeftProperty);
                    obRedFire1.dPosY = (double)Fire1.GetValue(Canvas.TopProperty);
                }
                // Modificar coordenadas del indicador que refleja el movimiento (Ellipse rojo)
                Puntero.SetValue(Canvas.TopProperty, dMano_Y - 12.5);
                Puntero.SetValue(Canvas.LeftProperty, dMano_X - 12.5);

                

                ////Puntero
                obPuntero.dPosX = (double)Puntero.GetValue(Canvas.LeftProperty);
                obPuntero.dPosY = (double)Puntero.GetValue(Canvas.TopProperty);
                obPuntero.dAncho = Puntero.Width;
                obPuntero.dAlto = Puntero.Height;

                

                // Indicar Id de la persona que es trazada
                LID.Content = skeleton.TrackingId;

                Console.WriteLine("LLega aqui");
                if (checarColisionFuego(obPuntero, obRedFire1) && obRedFire1.colisionando == false)
                {
                    Console.WriteLine("entro");
                   
                    obRedFire1.colisionando = true;
                    obRedFire1.dPosX = (double)Fire1.GetValue(Canvas.LeftProperty);
                    obRedFire1.dPosY = (double)Fire1.GetValue(Canvas.TopProperty);
                }
                else if (checarColisionFuego(obPuntero, obRedFire2) && obRedFire2.colisionando == false)
                {
                    obRedFire2.colisionando = true;
                    obRedFire2.dPosX = (double)Fire2.GetValue(Canvas.LeftProperty);
                    obRedFire2.dPosY = (double)Fire2.GetValue(Canvas.TopProperty);
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

            return true;
        }
        private void InitializeMonstersFire()
        {
            Random rnd = new Random();

            //Inicio las imagenes
            obAfraidRed1.dPosX = (double)Afraid.GetValue(Canvas.LeftProperty);
            obAfraidRed1.dPosY = (double)Afraid.GetValue(Canvas.TopProperty);
            obAfraidRed1.dAlto = Afraid.Height;
            obAfraidRed1.dAncho = Afraid.Width;
            obAfraidRed1.tipo = 1;
            obAfraidRed1.activo = false;
            Afraid.Visibility = Visibility.Collapsed;

           
            obAfraidWhite1.dPosX = (double)Afraidwhite.GetValue(Canvas.LeftProperty);
            obAfraidWhite1.dPosY = (double)Afraidwhite.GetValue(Canvas.TopProperty);
            obAfraidWhite1.dAlto = Afraidwhite.Height;
            obAfraidWhite1.dAncho = Afraidwhite.Width;
            obAfraidWhite1.tipo = 2;
            obAfraidWhite1.activo = false;
            Afraidwhite.Visibility = Visibility.Collapsed;
            
            obAfraidYellow1.dPosX = (double)Afraidyellow.GetValue(Canvas.LeftProperty);
            obAfraidYellow1.dPosY = (double)Afraidyellow.GetValue(Canvas.TopProperty);
            obAfraidYellow1.dAlto = Afraidyellow.Height;
            obAfraidYellow1.dAncho = Afraidyellow.Width;
            obAfraidYellow1.tipo = 3;
            obAfraidYellow1.activo = false;
            Afraidyellow.Visibility = Visibility.Collapsed;

            //// inicio en random la location del Fuego 
            //Fire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));
            //Fire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
            obRedFire1.dPosX = (double)Fire1.GetValue(Canvas.LeftProperty);
            obRedFire1.dPosY = (double)Fire1.GetValue(Canvas.TopProperty);
            obRedFire1.dAlto = Fire1.Height;
            obRedFire1.dAncho = Fire1.Width;
            obRedFire1.tipo = 1;
            obRedFire1.colisionando = false;



            Fire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(688, 785 - 70));
            Fire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));
            obRedFire2.dPosX = (double)Fire2.GetValue(Canvas.LeftProperty);
            obRedFire2.dPosY = (double)Fire2.GetValue(Canvas.TopProperty);
            obRedFire2.tipo = 1;

        }
        private void UpdateMonsters()
        {

            Afraid.SetValue(Canvas.LeftProperty, obAfraidRed1.dPosX);
            Afraid.SetValue(Canvas.TopProperty, obAfraidRed1.dPosY);
            //Afraid.SetValue(HeightProperty, obAfraidRed1.dAlto);

            if (obAfraidRed1.activo)
            {
                Afraid.Height *= 1.1;
                Afraid.Width *= 1.1;
                obAfraidRed1.dAlto = Afraid.Height;
                obAfraidRed1.dAncho = Afraid.Width;
                obAfraidRed1.dPosX = (double)Afraid.GetValue(LeftProperty);
                obAfraidRed1.dPosY = (double)Afraid.GetValue(TopProperty);
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
            }
           

            Afraidwhite.SetValue(Canvas.LeftProperty, obAfraidYellow1.dPosX);
            Afraid.SetValue(Canvas.TopProperty, obAfraidYellow1.dPosY);

            if (obAfraidYellow1.activo)
            {
                Afraidyellow.Height *= 1.1;
                Afraidyellow.Width *= 1.1;
                obAfraidYellow1.dAlto = Afraidyellow.Height;
                obAfraidYellow1.dAncho = Afraidyellow.Width;
                obAfraidYellow1.dPosX = (double)Afraidyellow.GetValue(LeftProperty);
                obAfraidYellow1.dPosY = (double)Afraidyellow.GetValue(TopProperty);
                
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
