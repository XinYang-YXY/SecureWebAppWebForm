<%@ Page Title="Register Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SecureWebAppWebForm._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container register rounded">
        <div class="row">
            <div class="col-md-3 register-left">
                <img src="https://image.ibb.co/n7oTvU/logo_white.png" alt="" />
                <h3>Welcome To  SITConnect</h3>
                <p>The place where you purchase stationary online!</p>
                <a class="nav-link" runat="server" href="~/Login">Login</a>
                <br />

            </div>
            <div class="col-md-8 col-11 register-right rounded">
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active" id="home" role="tabpanel" aria-labelledby="home-tab">
                        <h3 class="register-heading">Registration Form</h3>
                        <div class="row register-form">
                            <div class="col-md-6 ">
                                <div class="form-group ">
                                    <asp:TextBox type="text" class="form-control w-100" placeholder="First Name *" value="" ID="TBRegisterFirstName" runat="server" required="true"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox type="email" class="form-control" placeholder="Your Email *" value="" ID="TBRegisterEmail" runat="server" required="true"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox type="tel" class="form-control" placeholder="MM / YY *" value="" ID="TBRegisterCreditCardDate" runat="server" required="true"></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <asp:TextBox TextMode="Password" class="form-control" placeholder="Password *" value="" ID="TBRegisterPassword" runat="server" onkeyup="javascript:validate()" required="true"></asp:TextBox>
                                    <asp:Label ID="LBLRegisterPasswordAlert" runat="server" CssClass="text-danger"></asp:Label></div>
                            </div>
                            <div class="col-md-6 ">
                                <div class="form-group">
                                    <asp:TextBox type="text" class="form-control" placeholder="Last Name *" value="" ID="TBRegisterLastName" runat="server" required="true"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox type="tel" class="form-control" placeholder="Credit Card Number *" value="" ID="TBRegisterCreditCardNum" runat="server" required="true"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox type="tel" class="form-control" placeholder="CVC *" value="" ID="TBRegisterCVC" runat="server" required="true"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox type="password" class="form-control" placeholder="Confirm Password *" value="" ID="TBRegisterConfirmPassword" runat="server" required="true"></asp:TextBox>
                                </div>

                            </div>
                            <div class="form-group col-md-12">
                                <asp:TextBox type="date" class="form-control col-md-12" value="" ID="TBRegisterDOB" runat="server" required="true"></asp:TextBox>
                            </div>
                            <input type="hidden" id="g-recaptcha-response-register" name="g-recaptcha-response-register"/>
                            <asp:Label ID="registerCaptcha" runat="server" Text=""></asp:Label>
                            <asp:Button type="submit" class="btnRegister" ID="BTNRegisterSubmit" runat="server" Text="Register" OnClick="RegisterSubmitBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script type="text/javascript">
            function validate() {
                var password = document.getElementById('<%=TBRegisterPassword.ClientID %>').value;

                if (password.length < 8) {
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Password Length must be at least 8 characters";
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-danger";
                    return ("Password too short")
                } else if (password.search(/[0-9]/) == -1) {
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Password needs at least 1 number";
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-danger";
                    return ("no_number")
                } else if (password.search(/[A-Z]/) == -1) {
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Password requires at least 1 upper case";
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-danger";
                    return ("no_uppercase")
                } else if (password.search(/[a-z]/) == -1) {
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Password requires at least 1 lower case";
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-danger";
                    return ("no_lowercase");
                }
                else if (password.search(/[^A-Za-z0-9]/) == -1) {
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Password requires at least 1 special character";
                    document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-danger";
                    return ("no_specialChar");
                }
                document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").innerHTML = "Excellent Password!";
                document.getElementById("<%=LBLRegisterPasswordAlert.ClientID %>").className = "text-success";


            }
        </script>
         <script>grecaptcha.ready(function () { grecaptcha.execute('6Le3TkgaAAAAAOH6LIQ8sRzASt4PNG8b8MIDI0m6', { action: 'Register' }).then(function (token) { document.getElementById("g-recaptcha-response-register").value = token; }); });       </script>
</asp:Content>

