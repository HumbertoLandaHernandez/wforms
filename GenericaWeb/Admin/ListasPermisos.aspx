<%@ Page Title="Listas de permisos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListasPermisos.aspx.cs" Inherits="GenericaWeb.Admin.ListasPermisos" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Listas de permisos</h2>
    <p class="text-muted">Una lista (<code>Sistema.PermisoLista</code>) agrupa permisos (<code>Sistema.PermisoListaElemento</code>)
        y se asigna a cada rol (<em>RolPermisosId</em>). Los permisos de un usuario son los de las listas de todos sus roles
        más su permiso individual.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-7">
            <asp:GridView ID="gvListas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvListas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="PermisoListaId" HeaderText="Id" />
                    <asp:BoundField DataField="PermisoListaDesc" HeaderText="Lista" />
                    <asp:BoundField DataField="Permisos" HeaderText="Permisos" />
                    <asp:BoundField DataField="Roles" HeaderText="Roles" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("PermisoListaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("PermisoListaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar la lista de permisos?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay listas de permisos.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-5">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nueva lista" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-2">
                    <label class="form-label">Descripción</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDesc" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <label class="form-label">Permisos de la lista</label>
                <div class="border rounded p-2 mb-3 lista-paginas">
                    <asp:CheckBoxList ID="cblPermisos" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
