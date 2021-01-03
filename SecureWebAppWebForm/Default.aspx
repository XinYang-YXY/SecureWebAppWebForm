<%@ Page Title="Register Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SecureWebAppWebForm._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container register rounded">
        <div class="row">
            <div class="col-md-3 register-left">
                <img src="https://image.ibb.co/n7oTvU/logo_white.png" alt="" />
                <h3>Welcome To  SITConnect</h3>
                <p>The place where you purchase stationary online!</p>
                <a class="nav-link" runat="server" href="~/Login">Login</a> <br />

            </div>
            <div class="col-md-8 col-11 register-right rounded">
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active" id="home" role="tabpanel" aria-labelledby="home-tab">
                        <h3 class="register-heading">Registration Form</h3>
                        <div class="row register-form">
                            <div class="col-md-6 ">
                                <div class="form-group ">
                                    <input type="text" class="form-control w-100" placeholder="First Name *" value="" />
                                </div>
                                <div class="form-group">
                                    <input type="email" class="form-control" placeholder="Your Email *" value="" />
                                </div>
                                <div class="form-group">
                                    <input type="tel" class="form-control" placeholder="MM / YY *" value="" />
                                </div>

                                <div class="form-group">
                                    <input type="password" class="form-control" placeholder="Password *" value="" />
                                </div>
                            </div>
                            <div class="col-md-6 ">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Last Name *" value="" />
                                </div>
                                <div class="form-group">
                                    <input type="tel" class="form-control" placeholder="Credit Card Number *" value="" />
                                </div>
                                <div class="form-group">
                                    <input type="tel" class="form-control" placeholder="CVC *" value="" />
                                </div>
                                <div class="form-group">
                                    <input type="password" class="form-control" placeholder="Confirm Password *" value="" />
                                </div>

                            </div>
                            <div class="form-group col-md-12">
                                <input type="date" class="form-control col-md-12" value="" />
                            </div>
                            <input type="submit" class="btnRegister" value="Register" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
