// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ServiciosCognitivos
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton botonAlerta { get; set; }

        [Outlet]
        UIKit.UIButton botonNotificaciones { get; set; }

        [Outlet]
        UIKit.UIButton btnDescripccion { get; set; }

        [Outlet]
        UIKit.UIButton btnFelicidad { get; set; }

        [Outlet]
        UIKit.UIImageView imagen { get; set; }

        [Outlet]
        UIKit.UITextView lblDescripccion { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (botonAlerta != null) {
                botonAlerta.Dispose ();
                botonAlerta = null;
            }

            if (botonNotificaciones != null) {
                botonNotificaciones.Dispose ();
                botonNotificaciones = null;
            }

            if (btnDescripccion != null) {
                btnDescripccion.Dispose ();
                btnDescripccion = null;
            }

            if (btnFelicidad != null) {
                btnFelicidad.Dispose ();
                btnFelicidad = null;
            }

            if (imagen != null) {
                imagen.Dispose ();
                imagen = null;
            }

            if (lblDescripccion != null) {
                lblDescripccion.Dispose ();
                lblDescripccion = null;
            }
        }
    }
}
