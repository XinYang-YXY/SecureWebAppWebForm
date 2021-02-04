<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PasswordReset.aspx.cs" Inherits="SecureWebAppWebForm.PasswordReset" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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
                        <h3 class="register-heading">Reset Password</h3>
                        <div class="row register-form">
                            <div class="col-md-12 center-block">
                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="email" class="form-control" placeholder="Your Email *" value="" ID="TBResetEmail" runat="server"></asp:TextBox>
                                </div>

                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="password" class="form-control" placeholder="Old Password *" value="" runat="server" ID="TBResetOldPassword"></asp:TextBox>
                                </div>
                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="password" class="form-control" placeholder="New Password *" value="" runat="server" ID="TBResetNewPassword"></asp:TextBox>
                                </div>
                                <div class="form-group d-flex justify-content-center">
                                    <asp:TextBox type="password" class="form-control" placeholder="Confirm New Password *" value="" runat="server" ID="TBResetNewPasswordConfirm"></asp:TextBox>
                                </div>
                            <asp:Label ID="resetErrorMsg" runat="server" Text="cc" Visible="False"></asp:Label>
                            <asp:Button runat="server" class="btnRegister" ID="BTNResetSubmit" Text="Reset" OnClick="BTNResetSubmit_Click"  />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
