using System;
using Pki = Lacuna.Pki.Pades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace PkiSdkNetCoreMVCSample.Classes
{
	public class PadesVisualElements
	{

		#region PKI SDK

		// This function is called by the PAdES samples for PKI SDK. It contains a example of signature visual
		// representation. This is only a separate function in order to organize the variaous examples.
		public static Pki.PadesVisualRepresentation2 GetVisualRepresentationForPkiSdk(Lacuna.Pki.PKCertificate cert, IWebHostEnvironment env)
		{

			// Create a visual representation.
			var visualRepresentation = new Pki.PadesVisualRepresentation2()
			{

				// Text of the visual representation.
				Text = new Pki.PadesVisualText()
				{
					CustomText = String.Format("Signed by {0} ({1})", cert.SubjectDisplayName, cert.PkiBrazil.CPF),
					FontSize = 13.0,
					// Specify that the signing time should also be rendered.
					IncludeSigningTime = true,
					// Optionally set the horizontal alignment of the text ('Left' or 'Right'), if not set the
					// default is Left.
					HorizontalAlign = Pki.PadesTextHorizontalAlign.Left,
					// Optionally set the container within the signature rectangle on which to place the
					// text. By default, the text can occupy the entire rectangle (how much of the rectangle the
					// text will actually fill depends on the length and font size). Below, we specify that text
					// should respect a right margin of 1.5 cm.
					Container = new Pki.PadesVisualRectangle()
					{
						Left = 0.2,
						Top = 0.2,
						Right = 0.2,
						Bottom = 0.2
					}
				},
				Image = new Pki.PadesVisualImage()
				{
					// We'll use as background the image in Content/PdfStamp.png
					Content = StorageMock.GetPdfStampContent(env),
					// Align image to the right horizontally.
					HorizontalAlign = Pki.PadesHorizontalAlign.Right,
					// Align image to center vertically.
					VerticalAlign = Pki.PadesVerticalAlign.Center
				}
			};

			// Position of the visual representation. We get the footnote position preset and customize it.
			var visualPositioning = Pki.PadesVisualAutoPositioning.GetFootnote();
			visualPositioning.Container.Height = 4.94;
			visualPositioning.SignatureRectangleSize.Width = 8.0;
			visualPositioning.SignatureRectangleSize.Height = 4.94;
			visualRepresentation.Position = visualPositioning;

			return visualRepresentation;
		}

		#endregion

	}
}
