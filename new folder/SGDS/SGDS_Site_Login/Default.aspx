<%@ Page Title="IMDA SMEs Go Digital | Online Login" Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="SGDS_Internet_Appliation._Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>IMDA SMEs Go Digital | Online Login</title>
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/jQuery/jquery-ui.min.css" rel="stylesheet" />
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/Main.css" rel="stylesheet" />
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/footer.css" rel="stylesheet" />
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/sgds_landing_page.css" rel="stylesheet" />
    <link href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/css/sgds_activants.css" rel="stylesheet" />
    <link rel="shortcut icon" type="image/x-icon" href="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/images/icon-imda.ico" />
</head>
<body>
    <form id="SGDS_Master" runat="server">
        <div class="masthead-container" id="navbar">
            <nav class="navbar is-transparent full-width">
                <div class="sgds-container full-width col-lg-12">
                    <div class="navbar-brand">
                        <a class="navbar-item" href="<%= ConfigurationManager.AppSettings("IMDALogo") %>" target="_blank">
                            <img class="nav-image" alt="" src="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/Images/mda_logo.png" />
                        </a>
                    </div>
                    <div class="navbar-brand is-hidden-mobile">
                        <a class="navbar-item" href="<%= ConfigurationManager.AppSettings("SMEsDigitalLogo") %>">
                            <p>
                                <img class="nav-image2" alt="" src="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/Images/SGD SMEs Go Digital-Tier2a.jpg" />
                            </p>
                        </a>
                    </div>
                    <div class="navbar-brand navbar-end is-hidden-mobile">
                        <div class="wis-logo">
                            <a class="navbar-item" href="<%= ConfigurationManager.AppSettings("GovernmentLogo") %>" target="_blank">
                                <img class="nav-image" alt="" src="<%= ConfigurationManager.AppSettings("VirtualPath") %>/Content/Images/sg-logo.png" />
                            </a>
                        </div>
                        <ul class="wis-link">
                            <li><a href="<%= ConfigurationManager.AppSettings("ContactUs") %>" target="_blank">contact us</a></li>
                            <li><a href="<%= ConfigurationManager.AppSettings("FeedBack") %>" target="_blank">feedback</a></li>
                            <li><a href="<%= ConfigurationManager.AppSettings("SiteMap") %>" target="_blank">sitemap</a></li>
                            <li><a href="<%= ConfigurationManager.AppSettings("FAQ") %>" target="_blank">FAQ</a></li>
                        </ul>

                    </div>
                </div>
            </nav>
        </div>
        <div class="landing-page-body">
            <div class="landing-page-container">
                <div class="col-sm-12 col-md-12 col-lg-12">
                    <div id="divMaintenanceMessage">
                        <%= str_maintenance_message %>
                    </div>
                </div>
                <div class="logcontainer">
                    <div class="logblock" style="background-color: #d50867;">
                        <div class="col-sm-12 cp-landing-page" style="background-color: #78186c;">
                            <div class="col-sm-10">
                                <h4>Existing User</h4>
                            </div>
                        </div>
                        <div class="col-sm-12 cp-landing-page">
                            <div class="col-sm-12" style="margin:20px 0px 20px 0px;">Log-in as Organisation:</div>
                        </div>
                        <div class="col-sm-12 cp-landing-page">
                            <div class="col-sm-12" style="margin-bottom: 10px;text-align:center;">
                                <a href="<%= ConfigurationManager.AppSettings("CorpassOrganizationExistingUserLoginUrl")%>" class="btn btn-default form-control" role="button" id="btn_log">
                                    <strong>CorpPass</strong>
                                </a>
                            </div>
                        </div>
                    </div>
                    <div class="logblock" style="background-color: #2c7a83;">
                        <div class="col-sm-12 cp-landing-page" style="background-color: #78186c;">
                            <div class="col-sm-10">
                                <h4>New User</h4>
                            </div>
                        </div>
                        <div class="col-sm-12 cp-landing-page">
                            <div class="col-sm-12" style="margin:20px 0px 20px 0px;">Register as Organisation:</div>
                        </div>
                        <div class="col-sm-12 cp-landing-page">
                            <div class="col-sm-12" style="margin-bottom: 10px;text-align:center;">
                                <a href="<%= ConfigurationManager.AppSettings("CorpassOrganizationNewUserRegistrationUrl") %>" class="btn btn-default form-control" role="button" id="btn_register">
                                    <strong>CorpPass</strong>
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-12 col-lg-12">
                    <div id="divLandingNote">
                        <ul>
                            <li>From 1 September 2018, CorpPass will be the only login method for online corporate transactions with the Government. If you are accessing SMEs Go Digital System under an Organisation, you must first register an account with <b><a href="https://www.corppass.gov.sg/cpauth/login/homepage?TAM_OP=login" target="_blank">CorpPass</a></b> before you can login.</li>
                            <li>Please read the <b><a href="https://www.imda.gov.sg/terms-of-use" target="_blank">Terms of Use</a></b> before proceeding further.</li>
                            <li>In the event you encounter technical issues, please let us know via <b><a href="mailto:smes_go_digital@imda.gov.sg">SMEs_Go_Digital@imda.gov.sg</a></b> with detailed explanation and screenshots.</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <footer class="sgds-footer top-section">
            <div class="sgds-container is-fluid">
                <h5 class="sub-header has-text-white"><b>SMEs Go Digital</b></h5>
                <ul class="is-right-desktop-only">
                    <li class="is-inline-block-desktop-only"><a href="<%= ConfigurationManager.AppSettings("FooterContactUs") %>" target="_blank">Contact</a></li>
                    <li class="is-inline-block-desktop-only"><a href="<%= ConfigurationManager.AppSettings("FooterFAQ") %>" target="_blank">FAQ</a></li>
                    <li class="is-inline-block-desktop-only"><a href="<%= ConfigurationManager.AppSettings("FooterPrivacyStatement") %>" target="_blank">Privacy Statement</a></li>
                    <li class="is-inline-block-desktop-only"><a href="<%= ConfigurationManager.AppSettings("FooterTermsOfUse") %>" target="_blank">Terms of Use</a></li>
                </ul>
            </div>
        </footer>
        <footer class="sgds-footer bottom-section">
            <div class="sgds-container is-fluid">
                <div class="row is-vcentered divider">
                    <div class="col footer-col has-text-right-desktop has-text-left-mobile">
                        <p class="is-hidden-touch">
                            Copyright © <%= ConfigurationManager.AppSettings("SGD_Last_Updated_Copyright_Year") %> Info-communications Media Development Authority. All Rights Reserved. Last Updated <%= ConfigurationManager.AppSettings("SGD_Last_Updated_Date") %>
                        </p>
                        <p class="is-hidden-desktop">
                            Copyright © <%= ConfigurationManager.AppSettings("SGD_Last_Updated_Copyright_Year") %> Info-communications Media Development Authority. All Rights Reserved.
                        </p>
                        <p class="is-hidden-desktop last-updated">
                            Last Updated <%= ConfigurationManager.AppSettings("SGD_Last_Updated_Date") %>
                        </p>
                    </div>
                </div>
            </div>
        </footer>
    </form>
</body>
</html>
