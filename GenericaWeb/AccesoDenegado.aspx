<%@ Page Title="Acceso denegado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccesoDenegado.aspx.cs" Inherits="GenericaWeb.AccesoDenegado" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="alert alert-warning">
        <h4 class="alert-heading">Acceso denegado</h4>
        <p class="mb-0">No tiene permiso para ver la página solicitada. Solicite acceso al administrador.</p>
    </div>
    <a runat="server" href="~/Default.aspx" class="btn btn-primary">Ir al inicio</a>
</asp:Content>
