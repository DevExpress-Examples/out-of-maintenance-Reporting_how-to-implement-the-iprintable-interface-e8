Imports Microsoft.VisualBasic
Imports System
Namespace IPrintableImplementation
	Partial Public Class Form1
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.btnPrintPreview = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			' 
			' btnPrintPreview
			' 
			Me.btnPrintPreview.Location = New System.Drawing.Point(89, 83)
			Me.btnPrintPreview.Name = "btnPrintPreview"
			Me.btnPrintPreview.Size = New System.Drawing.Size(98, 47)
			Me.btnPrintPreview.TabIndex = 0
			Me.btnPrintPreview.Text = "Print Preview"
			Me.btnPrintPreview.UseVisualStyleBackColor = True
'			Me.btnPrintPreview.Click += New System.EventHandler(Me.btnPrintPreview_Click);
			' 
			' Form1
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(284, 262)
			Me.Controls.Add(Me.btnPrintPreview)
			Me.Name = "Form1"
			Me.Text = "Form1"
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private WithEvents btnPrintPreview As System.Windows.Forms.Button

	End Class
End Namespace

