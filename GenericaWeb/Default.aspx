<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GenericaWeb._Default" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Bienvenido, <asp:Literal ID="litNombre" runat="server" Mode="Encode" /></h2>
    <div class="row g-3">
        <div class="col-md-4">
            <div class="card">
                <div class="card-header">Sesión</div>
                <ul class="list-group list-group-flush">
                    <li class="list-group-item"><strong>Usuario:</strong> <asp:Literal ID="litUsuario" runat="server" Mode="Encode" /></li>
                    <li class="list-group-item"><strong>Roles:</strong> <asp:Literal ID="litRol" runat="server" Mode="Encode" /></li>
                    <li class="list-group-item"><strong>Permiso:</strong> <asp:Literal ID="litPermiso" runat="server" Mode="Encode" /></li>
                    <li class="list-group-item"><strong>Token expira (UTC):</strong> <asp:Literal ID="litExpira" runat="server" Mode="Encode" /></li>
                </ul>
            </div>
        </div>
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">Páginas disponibles</div>
                <div class="card-body">
                    <asp:Repeater ID="rptGrupos" runat="server">
                        <ItemTemplate>
                            <h6 class="mt-2"><%#: Eval("Titulo") %></h6>
                            <div class="d-flex flex-wrap gap-2 mb-2">
                                <asp:Repeater runat="server" DataSource='<%# Eval("Paginas") %>'>
                                    <ItemTemplate>
                                        <a class="btn btn-outline-primary btn-sm" href="<%#: ResolveUrl((string)Eval("Ruta")) %>"><%#: Eval("Titulo") %></a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="litSinPaginas" runat="server" Visible="false" Text="No tiene páginas asignadas en el menú." />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
