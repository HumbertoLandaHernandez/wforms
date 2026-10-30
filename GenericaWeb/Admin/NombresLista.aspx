<%@ Page Title="Nombres completos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NombresLista.aspx.cs" Inherits="GenericaWeb.Admin.NombresLista" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Nombres completos</h2>
    <p class="text-muted">Cada nombre completo (<code>Sistema.NombreLista</code>) se forma con elementos
        (<code>Sistema.NombreListaElemento</code>): tipo de nombre + nombre. Se asigna a cada usuario (<em>UsuarioNombreId</em>).</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-6">
            <asp:GridView ID="gvListas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvListas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NombreListaId" HeaderText="Id" />
                    <asp:BoundField DataField="NombreListaDesc" HeaderText="Nombre completo" />
                    <asp:BoundField DataField="Elementos" HeaderText="Elementos" />
                    <asp:BoundField DataField="NombreListaUsys" HeaderText="Modificó" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("NombreListaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("NombreListaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar el nombre completo y sus elementos?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay nombres registrados.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-6">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body mb-3" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nuevo nombre completo" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-3">
                    <label class="form-label">Descripción (nombre que se muestra)</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" MaxLength="50" ValidationGroup="Encabezado" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDesc" ErrorMessage="Requerido" ValidationGroup="Encabezado"
                                                CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" ValidationGroup="Encabezado" />
                    <asp:Button ID="btnComponer" runat="server" Text="Componer desde elementos" CssClass="btn btn-outline-secondary"
                                OnClick="btnComponer_Click" CausesValidation="false" Visible="false" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlElementos" runat="server" CssClass="card card-body" Visible="false" DefaultButton="btnAgregar">
                <h6>Elementos</h6>
                <asp:GridView ID="gvElementos" runat="server" AutoGenerateColumns="false" GridLines="None"
                              CssClass="table table-sm mb-3" OnRowCommand="gvElementos_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="NombreTipoDesc" HeaderText="Tipo" />
                        <asp:BoundField DataField="NombreDesc" HeaderText="Nombre" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Quitar"
                                                CommandArgument='<%# Eval("NombreTipoId") + "|" + Eval("NombreId") %>'
                                                CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'>Quitar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>Sin elementos.</EmptyDataTemplate>
                </asp:GridView>
                <asp:PlaceHolder ID="phAgregar" runat="server">
                    <div class="row g-2 align-items-end">
                        <div class="col-sm-5">
                            <label class="form-label">Tipo</label>
                            <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTipo" InitialValue="" ErrorMessage="Requerido"
                                                        ValidationGroup="Elemento" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-sm-5">
                            <label class="form-label">Nombre</label>
                            <asp:DropDownList ID="ddlNombre" runat="server" CssClass="form-select" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNombre" InitialValue="" ErrorMessage="Requerido"
                                                        ValidationGroup="Elemento" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-sm-2">
                            <asp:Button ID="btnAgregar" runat="server" Text="Agregar" CssClass="btn btn-primary w-100"
                                        OnClick="btnAgregar_Click" ValidationGroup="Elemento" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
