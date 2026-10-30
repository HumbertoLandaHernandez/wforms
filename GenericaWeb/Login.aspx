<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GenericaWeb.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Iniciar sesión - Genérica</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/site.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="d-flex align-items-center justify-content-center min-vh-100">
        <asp:Panel ID="pnlLogin" runat="server" CssClass="card shadow-sm login-card" DefaultButton="btnEntrar">
            <div class="card-body p-4">
                <h1 class="h4 mb-4 text-center">Genérica</h1>
                <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block" Visible="false" />
                <div class="mb-3">
                    <label for="txtUsuario" class="form-label">Usuario</label>
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" MaxLength="30" autocomplete="username" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsuario" ErrorMessage="Capture el usuario."
                                                CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label for="txtContrasena" class="form-label">Contraseña</label>
                    <asp:TextBox ID="txtContrasena" runat="server" CssClass="form-control" TextMode="Password" autocomplete="current-password" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtContrasena" ErrorMessage="Capture la contraseña."
                                                CssClass="text-danger small" Display="Dynamic" />
                </div>
                <asp:Button ID="btnEntrar" runat="server" Text="Entrar" CssClass="btn btn-primary w-100" OnClick="btnEntrar_Click" />
            </div>
        </asp:Panel>
    </form>
</body>
</html>
