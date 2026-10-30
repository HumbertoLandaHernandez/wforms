<%@ Page Title="Catálogos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Catalogos.aspx.cs" Inherits="GenericaWeb.Admin.Catalogos" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Catálogos</h2>
    <ul class="nav nav-tabs mb-3">
        <asp:Repeater ID="rptCatalogos" runat="server" EnableViewState="false">
            <ItemTemplate>
                <li class="nav-item">
                    <a class='nav-link <%# (string)Eval("Clave") == Definicion.Clave ? "active" : "" %>'
                       href='Catalogos.aspx?t=<%#: Eval("Clave") %>'><%#: Eval("Titulo") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-7">
            <asp:GridView ID="gvCatalogo" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvCatalogo_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" />
                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                    <asp:BoundField DataField="Usys" HeaderText="Modificó" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar el registro?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>Sin registros.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-5">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nuevo registro" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-3">
                    <label class="form-label">Descripción</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDesc" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
