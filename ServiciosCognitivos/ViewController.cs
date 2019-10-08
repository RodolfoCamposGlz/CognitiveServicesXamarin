using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using AVFoundation;
using UIKit;
using Foundation;

namespace ServiciosCognitivos
{
    public partial class ViewController : UIViewController
    {
		AVSpeechSynthesizer habla = new AVSpeechSynthesizer();
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			botonAlerta.TouchUpInside += delegate
			{
				var Alerta = UIAlertController.Create("Alerta", "Ejemplo de alerta en iOS",
													  UIAlertControllerStyle.Alert);
				Alerta.AddAction(UIAlertAction.Create
								 ("ok", UIAlertActionStyle.Default, alert =>
								  Console.WriteLine("Afirmativo")));
				Alerta.AddAction(UIAlertAction.Create
								 ("Cancel", UIAlertActionStyle.Cancel, alert =>
								  Console.WriteLine("Negativo")));
				PresentViewController(Alerta, true, null);

			};
            botonNotificaciones.TouchUpInside += delegate
			{
				var notification = new UILocalNotification();
				notification.FireDate = NSDate.FromTimeIntervalSinceNow(5);
				notification.AlertAction = "Notificacion";
				notification.AlertBody = "Tienes una notifiacion";
				notification.SoundName = UILocalNotification.DefaultSoundName;
				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
			};
			btnFelicidad.TouchUpInside += Felicidad;
			btnDescripccion.TouchUpInside += Descripcion;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        async void Felicidad (object sender, EventArgs e)
        {
            var ruta = await DescargarImagenAnalizarFelicidad();
            imagen.Image = UIImage.FromFile(ruta);
            var StreamdeImagen = imagen.Image.AsJPEG(.5f).AsStream();
            {
                try
                {
                    float porcentaje = await NiveldeFelicidad(StreamdeImagen);
                    lblDescripccion.Text = TraerMensajeEmocion(porcentaje);
                    var voz = new AVSpeechUtterance(lblDescripccion.Text);
                    voz.Voice = AVSpeechSynthesisVoice.FromLanguage("es-MX");
                    habla.SpeakUtterance(voz);

                }
                catch (Exception ex)
                {
                    lblDescripccion.Text = ex.Message;
                }
            }
        }
        public async Task<string> DescargarImagenAnalizarFelicidad()
        {
            WebClient cliente = new WebClient();
            byte[] DatosImagen = await cliente.DownloadDataTaskAsync("https://menteurbana.mx/wp-content/uploads/2016/09/Playa-Delfines.jpg");
																		   //("https://s-media-cache-ak0.pinimg.com/736x/cc/cd/fc/cccdfc8b40683b7ca6bbd3ec477da023.jpg");

			string RutaLocal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string NombredeArchivo = "foto1.jpg";
            string ruta = Path.Combine(RutaLocal, NombredeArchivo);
            File.WriteAllBytes(ruta, DatosImagen);
            return ruta;

        }
        public static async Task<float> NiveldeFelicidad (Stream stream)
        {
            Emotion[] Emociones = await TraerEmocion(stream);
            float nivel = 0;
            foreach (var emocion in Emociones)
            {
                nivel = nivel + emocion.Scores.Happiness;
            }
            return nivel / Emociones.Count();
        }
        private static async Task<Emotion[]> TraerEmocion(Stream stream)
        {
            string ClaveAPIEmocion = "15bdbbb6269241a09068e2ef0c1d8fa8";
            EmotionServiceClient ClienteAPIEmocion = new EmotionServiceClient(ClaveAPIEmocion);
            var emocion = await ClienteAPIEmocion.RecognizeAsync(stream);
            if(emocion == null || emocion.Count()==0)
            {
                throw new Exception("No se pudo realizar deteccion");
            }
            return emocion;
        }
        public static string TraerMensajeEmocion(float nivel)
        {
            nivel = nivel * 100;
            double resultado = Math.Round(nivel, 2);
            if (nivel >= 60)
            {
                return "Es feliz, tiene una sonrisa de un :" + resultado + "%";
            }
            else 
            {
                return "Necesita un abrazo, tiene solo:" + resultado + "% de felicidad";
            }
        }
        async void Descripcion (object sender, EventArgs e)
        {
            var ruta = await DescargarImagenAnalizarFelicidad();
            imagen.Image = UIImage.FromFile(ruta);
            var StreamdeImagen = imagen.Image.AsJPEG(.5f).AsStream();
            {
                try
                {
                    var result = await DescripciondeImagen(StreamdeImagen);
                    imagen.Dispose();
                    lblDescripccion.Text="";
                    foreach (string tags in result.Description.Tags)
                    {
                        var voz = new AVSpeechUtterance(tags);
                        lblDescripccion.Text = lblDescripccion.Text + "\n" + tags;
                        habla.SpeakUtterance(voz);
                    }

                }
                catch (Exception ex)
                {
                    lblDescripccion.Text = ex.Message;
                }
            }
        }
        public async Task<string> DescargarImagenDescriccion()
        {
            WebClient cliente = new WebClient();
            byte[] DatosdeImagen = await cliente.DownloadDataTaskAsync("https://dl.dropboxusercontent.com/u/95408124/foto1.jpg");
            string rutalocal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string nombrearchivo = "foto2.jpg";
            string ruta = Path.Combine(rutalocal, nombrearchivo);
            File.WriteAllBytes(ruta,DatosdeImagen);
            return ruta;

        }
        public async Task<AnalysisResult> DescripciondeImagen(Stream StreamdeImagen)
        {
            VisionServiceClient ClienteAPIVision = new VisionServiceClient("16469763b34b4286be2ae646c5091bdb");
            VisualFeature[] Caracteristicas = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description };
            return await ClienteAPIVision.AnalyzeImageAsync(StreamdeImagen, Caracteristicas.ToList(), null);

        }
    }
}
