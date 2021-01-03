<%@ Page Title="Login Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SecureWebAppWebForm._Default" %>

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
                                    <input type="email" class="form-control" placeholder="Your Email *" value="" />
                                </div>

                                <div class="form-group d-flex justify-content-center">
                                    <input type="password" class="form-control" placeholder="Password *" value="" />
                                </div>
                            </div>
                            <input type="submit" class="btnRegister" value="Register" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
