<%@ Page Title="Login Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SecureWebAppWebForm.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container register rounded">
        <div class="row">
            <div class="col-md-3 register-left">
                <img src="https://image.ibb.co/n7oTvU/logo_white.png" alt="" />
                <h3>Welcome To  SITConnect</h3>
                <p>The place where you purchase stationary online!</p>
                <a class="nav-link" runat="server" href="~/Default">Register</a>
                <br />
            </div>
            <div class="col-md-8 col-11 register-right rounded">
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active" id="home" role="tabpanel" aria-labelledby="home-tab">
                        <h3 class="register-heading">Login</h3>
                        <div class="row register-form">
                            <div class="col-md-12 center-block">
                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="email" class="form-control" placeholder="Your Email *" value=""  ID="TBLoginEmail" runat="server"></asp:TextBox>
                                </div>

                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="password" class="form-control" placeholder="Password *" value="" runat="server" ID="TBLoginPassword"></asp:TextBox>
                                    <asp:Label ID="lblLoginPasswordAlerts" runat="server"></asp:Label></div>
                            </div>
                            <asp:Label ID="loginCaptcha" runat="server" Text=""></asp:Label>
                            <%--<div class="g-recaptcha" data-sitekey="6LfjzOQZAAAAAIb1M_33LPzD8asmsl3xL801Fy57"></div>--%>
                           <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                            <asp:Label ID="loginErrorMsg" runat="server" Text="" Visible="False"></asp:Label>
                            <asp:Button ID="GoToResetPasswordBtn" runat="server" Text="Reset Password" Visible="False" OnClick="GoToResetPasswordBtn_Click" />
                            <asp:Button runat="server"  class="btnRegister" ID="BTNLoginSubmit" Text="Login" OnClick="LoginSubmitBtn_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
