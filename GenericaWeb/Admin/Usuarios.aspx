<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="GenericaWeb.Admin.Usuarios" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Usuarios</h2>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-xl-8">
            <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvUsuarios_RowCommand">
                <Columns>
                    <asp:BoundField DataField="UsuarioId" HeaderText="Id" />
                    <asp:BoundField DataField="UsuarioDesc" HeaderText="Usuario" />
                    <asp:BoundField DataField="NombreListaDesc" HeaderText="Nombre" />
                    <asp:BoundField DataField="RolDesc" HeaderText="Rol principal" />
                    <asp:BoundField DataField="RolesAdicionales" HeaderText="Otros roles" />
                    <asp:BoundField DataField="PermisoDesc" HeaderText="Permiso" />
                    <asp:BoundField DataField="PaginaListaDesc" HeaderText="Lista de páginas" />
                    <asp:BoundField DataField="PaginaHome" HeaderText="Inicio" />
                    <asp:BoundField DataField="UsuarioDT" HeaderText="Modificado" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("UsuarioId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("UsuarioId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar el usuario?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay usuarios registrados.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-xl-4">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nuevo usuario" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-2">
                    <label class="form-label">Usuario</label>
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" MaxLength="30" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsuario" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Contraseña <small class="text-muted"><asp:Literal ID="litAyudaContrasena" runat="server" Text="(mínimo 8 caracteres)" /></small></label>
                    <asp:TextBox ID="txtContrasena" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Nombre completo</label>
                    <asp:DropDownList ID="ddlNombre" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNombre" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Rol principal</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlRol" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Otros roles <small class="text-muted">(Sistema.UsuarioRol)</small></label>
                    <div class="border rounded p-2 lista-paginas">
                        <asp:CheckBoxList ID="cblRoles" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical" />
                    </div>
                </div>
                <div class="mb-2">
                    <label class="form-label">Permiso individual</label>
                    <asp:DropDownList ID="ddlPermiso" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlPermiso" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Lista de páginas permitidas</label>
                    <asp:DropDownList ID="ddlPermisosPaginas" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlPermisosPaginas" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Página de inicio</label>
                    <asp:DropDownList ID="ddlPaginaHome" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlPaginaHome" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
