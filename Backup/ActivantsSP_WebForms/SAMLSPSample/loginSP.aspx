﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="loginSP.aspx.cs" Inherits="SAMLSPSample.loginSP" %>

<!DOCTYPE html>
<html>
<head>
<meta name="viewport" content="width=device-width, initial-scale=1">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
        <!-- Our Custom CSS -->
        <link rel="stylesheet" href="style.css">
         <script src="https://code.jquery.com/jquery-1.12.0.min.js"></script>
         <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<style>
    body{
        color:black;
    }
    .container-head{
        background-color:#FDD100;
        width: 100%;
        padding:30px;
    }

    .navbar-header{
        margin-left:110px !important;
    }
    .navbar-brand{
        padding:0px;
    }
    #navFontSize
    {
        font-family:'Open Sans Condensed';
        font-size:16px;
        font-weight:bold;
    }
    .jumbotron{
        background-image: url(images/header.jpg);
        color:white;
    }
    #ssoLinkButton{
        background-color:white !important;
        color:darkmagenta !important;
        border-color:transparent;
        padding:15px;
        font-size:16px;
        margin-top:10%;
    }
</style>
<body>
<nav class="navbar navbar-expand-md">
    <div class="container container-head">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#exampleNavComponents">
                <i class="glyphicon glyphicon-align-center"></i>
            </button>
            <a href="#" class="navbar-brand">
                <img src="Images/logo-mini.png"/>
            </a>
        </div>

        <div class="collapse navbar-collapse" id="exampleNavComponents">

            <ul class="nav navbar-nav navbar-right" id="navFontSize">
                <li class="active" >
                    <a href="#"><span style="color:darkmagenta"> PAGE1 </span></a>
                </li>
                <li class="active" >
                    <a href="#"><span style="color:darkmagenta"> PAGE2 </span></a>
                </li>
                 <li class="active" >
                    <a href="#"><span style="color:darkmagenta"> PAGE3 </span></a>
                </li>
                 <li class="active" >
                    <a href="#"><span style="color:darkmagenta"> PAGE4 </span></a>
                </li>
            </ul>

        </div>
    </div>
</nav>
<div class="container">
  <div class="jumbotron">
       <div class="row">
        <center>
          <div class="col-sm-6">
              <img src="Images/service.png" style="width:20%"/><h3>Activants Pte Ltd Service Provider</h3>
          </div>
          <div class="col-sm-6">
                <form action="#" runat="server">
                  <asp:Button  class="btn btn-primary" ID="ssoLinkButton" runat="server" Text="Initiate SAML (SSO) Login Request" OnClick="ssoLinkButton_Click" />
                </form>
          </div>
        </center>
        </div> 
  </div>     
</div>
</body>
</html>