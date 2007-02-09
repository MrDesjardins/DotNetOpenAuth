/********************************************************
 * Copyright (C) 2007 Andrew Arnott
 * Released under the Apache License 2.0
 * License available here: http://www.apache.org/licenses/LICENSE-2.0
 * For news or support on this file: http://jmpinline.nerdbank.net/
 ********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

namespace NerdBank.OpenId
{
	[DefaultProperty("OpenIdUrl")]
	[ToolboxData("<{0}:OpenIdLogin runat=\"server\"></{0}:OpenIdLogin>")]
	public class OpenIdLogin : OpenIdTextBox
	{
		Panel panel;
		Button loginButton;
		HtmlGenericControl label;
		Label examplePrefixLabel;
		Label exampleUrlLabel;
		HyperLink registerLink;

		protected override void CreateChildControls()
		{
			// Don't call base.CreateChildControls().  This would add the WrappedTextBox
			// to the Controls collection, which would implicitly remove it from the table
			// we have already added it to.
			
			// Just add the panel we've assembled earlier.
			Controls.Add(panel);

			if (ShouldBeFocused)
				WrappedTextBox.Focus();
		}

		protected override void InitializeControls()
		{
			base.InitializeControls();

			panel = new Panel();

			Table table = new Table();
			TableRow row1, row2;
			TableCell cell;
			table.Rows.Add(row1 = new TableRow());
			table.Rows.Add(row2 = new TableRow());

			// top row, left cell
			cell = new TableCell();
			label = new HtmlGenericControl("label");
			label.InnerText = labelTextDefault;
			cell.Controls.Add(label);
			row1.Cells.Add(cell);

			// top row, middle cell
			cell = new TableCell();
			cell.Controls.Add(WrappedTextBox);
			row1.Cells.Add(cell);

			// top row, right cell
			cell = new TableCell();
			loginButton = new Button();
			loginButton.ID = "loginButton";
			loginButton.Text = buttonTextDefault;
			loginButton.ToolTip = buttonToolTipDefault;
			loginButton.Click += new EventHandler(loginButton_Click);
#if !Mono
			panel.DefaultButton = loginButton.ID;
#endif
			cell.Controls.Add(loginButton);
			row1.Cells.Add(cell);

			// bottom row, left cell
			row2.Cells.Add(new TableCell());

			// bottom row, middle cell
			cell = new TableCell();
			cell.Style[HtmlTextWriterStyle.Color] = "gray";
			cell.Style[HtmlTextWriterStyle.FontSize] = "smaller";
			examplePrefixLabel = new Label();
			examplePrefixLabel.Text = examplePrefixDefault;
			cell.Controls.Add(examplePrefixLabel);
			cell.Controls.Add(new LiteralControl(" "));
			exampleUrlLabel = new Label();
			exampleUrlLabel.Font.Bold = true;
			exampleUrlLabel.Text = exampleUrlDefault;
			cell.Controls.Add(exampleUrlLabel);
			row2.Cells.Add(cell);

			// bottom row, right cell
			cell = new TableCell();
			cell.Style[HtmlTextWriterStyle.Color] = "gray";
			cell.Style[HtmlTextWriterStyle.FontSize] = "smaller";
			cell.Style[HtmlTextWriterStyle.TextAlign] = "center";
			registerLink = new HyperLink();
			registerLink.Text = registerTextDefault;
			registerLink.ToolTip = registerToolTipDefault;
			registerLink.NavigateUrl = registerUrlDefault;
			cell.Controls.Add(registerLink);
			row2.Cells.Add(cell);

			panel.Controls.Add(table);
		}

		protected override void RenderChildren(HtmlTextWriter writer)
		{
			if (!this.DesignMode)
				label.Attributes["for"] = WrappedTextBox.ClientID;

			base.RenderChildren(writer);
		}

		#region Properties
		const string labelTextDefault = "OpenID Login:";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(labelTextDefault)]
		[Localizable(true)]
		public string LabelText
		{
			get { return label.InnerText; }
			set { label.InnerText = value; }
		}

		const string examplePrefixDefault = "Example:";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(examplePrefixDefault)]
		[Localizable(true)]
		public string ExamplePrefix
		{
			get { return examplePrefixLabel.Text; }
			set { examplePrefixLabel.Text = value; }
		}

		const string exampleUrlDefault = "http://your.name.myopenid.com";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(exampleUrlDefault)]
		[Localizable(true)]
		public string ExampleUrl
		{
			get { return exampleUrlLabel.Text; }
			set { exampleUrlLabel.Text = value; }
		}

		const string registerTextDefault = "register";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(registerTextDefault)]
		[Localizable(true)]
		public string RegisterText
		{
			get { return registerLink.Text; }
			set { registerLink.Text = value; }
		}

		const string registerUrlDefault = "https://www.myopenid.com/signup";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(registerUrlDefault)]
		[Localizable(true)]
		public string RegisterUrl
		{
			get { return registerLink.NavigateUrl; }
			set { registerLink.NavigateUrl = value; }
		}

		const string registerToolTipDefault = "Sign up free for an OpenID with MyOpenID now.";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(registerToolTipDefault)]
		[Localizable(true)]
		public string RegisterToolTip
		{
			get { return registerLink.ToolTip; }
			set { registerLink.ToolTip = value; }
		}

		const string buttonTextDefault = "Login �";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(buttonTextDefault)]
		[Localizable(true)]
		public string ButtonText
		{
			get { return loginButton.Text; }
			set { loginButton.Text = value; }
		}

		const string buttonToolTipDefault = "Account login";
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(buttonToolTipDefault)]
		[Localizable(true)]
		public string ButtonToolTip
		{
			get { return loginButton.ToolTip; }
			set { loginButton.ToolTip = value;
			}
		}
		#endregion

		#region Event handlers
		void loginButton_Click(object sender, EventArgs e)
		{
			if (OnLoggingIn(new Uri(Text)))
				Login();
		}

		#endregion

		#region Events
		public event EventHandler<OpenIdTextBox.OpenIdEventArgs> LoggingIn;
		protected virtual bool OnLoggingIn(Uri openIdUri)
		{
			EventHandler<OpenIdTextBox.OpenIdEventArgs> loggingIn = LoggingIn;
			OpenIdTextBox.OpenIdEventArgs args = new OpenIdTextBox.OpenIdEventArgs(openIdUri,
				OpenIdTextBox.OpenIdProfileFields.Empty);
			if (loggingIn != null)
				loggingIn(this, args);
			return !args.Cancel;
		}
		#endregion
	}
}
