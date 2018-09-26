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


            Random rnd = new Random();

            //Inicio de localizacion de imagen
            Afraid.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
            Afraid.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
            Afraidwhite.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
            Afraidwhite.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));
            Afraidyellow.SetValue(Canvas.LeftProperty, (double)rnd.Next(220, 596));
            Afraidyellow.SetValue(Canvas.TopProperty, (double)rnd.Next(10, 506));

            // inicio en random la location de los globos
            Fire1.SetValue(Canvas.LeftProperty, (double)rnd.Next(15, 127 - 70));
            Fire1.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));

            Fire2.SetValue(Canvas.LeftProperty, (double)rnd.Next(688, 785 - 70));
            Fire2.SetValue(Canvas.TopProperty, (double)rnd.Next(0, 604 - 70));

            //Calcula	la	coordenada	del	centro	del	aro
            dXC = (double)Circulo2.GetValue(Canvas.LeftProperty) + (Circulo2.Width / 2);
            dYC = (double)Circulo2.GetValue(Canvas.TopProperty) + (Circulo2.Height / 2);

            //Calcular	el	radio	de	cada	uno	de	los	círculos
            dRadioC1 = Circulo1.Width / 2;
            dRadioC2 = Circulo2.Width / 2;
            // Realizar configuraciones e iniciar el Kinect

            Kinect_Config();
        }

        private void Growth_Tick(object sender, EventArgs e)
        {

            Afraid.Width = Afraid.Width * 1.1;
            Afraid.Height = Afraid.Height * 1.1;
            Afraidwhite.Width = Afraidwhite.Width * 1.1;
            Afraidwhite.Height = Afraidwhite.Height * 1.1;
            Afraidyellow.Width = Afraidyellow.Width * 1.1;
            Afraidyellow.Height = Afraidyellow.Height * 1.1;
        }

        private bool checarDistancia()
        {
            //Obtiene	la	coordenada	del	centro	del	círculo	que	mueve	la	persona
            double dX1 = (double)Puntero.GetValue(Canvas.LeftProperty) + (Puntero.Width / 2);
            double dY1 = (double)Puntero.GetValue(Canvas.TopProperty) + (Puntero.Height / 2);
            //Calcula	la	distancia	entre	el	centro	del	Puntero	(círculo	rojo)	y
            //el	centro	del	aro
            double dDistancia = Math.Sqrt(Math.Pow(dXC - dX1, 2) + Math.Pow(dYC - dY1, 2));
            //Compara	la	distancia	calculada	con	los	radios	de	los	dos	círculos	que	forman
            //el	aro	en	el	entendido	de	que	si	la	distancia	es	mayor	al	círculo	más	grande
            //o	menor	al	círculo	más	pequeño,	entonces	el	círculo	rojo	
            //se	ha	salido	del	trayecto.
            if (dDistancia > dRadioC1 || dDistancia < dRadioC2)

                return false;
            else
                return true;
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

                // Indicar Id de la persona que es trazada
                LID.Content = skeleton.TrackingId;
                if (checarDistancia())
                {
                    Circulo1.Fill = Brushes.Yellow; //No	se	encuentra
                }
                else
                {
                    Circulo1.Fill = Brushes.Black;      //Sí	se	encuentra
                }
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
