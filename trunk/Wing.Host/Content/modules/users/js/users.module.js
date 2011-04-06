Loader.run("UsersModuleLoader", function ()
{
    Command.register("user.logout", {
        text: "Sair",
        iconClass: "ui-icon-close",
        hint: "Encerrar a sessão atual",
        group: "current_user",
        order: 9999,
        execute: function ()
        {
            window.location = '/Login/Signout';
        }
    });

    Command.register("user.profile.show", {
        text: "Perfil...",
        iconClass: "ui-icon-person",
        hint: "Editar informações pessoais",
        order: 1,
        group: "current_user",
        execute: function ()
        {
            var dialog = new wing.user.ProfileDialog({ user: User });
            dialog.open(true);
        }
    });


    Type.define("wing.user.ProfileDialog", "wui.Dialog", {
        methods: {
            init: function ()
            {
                this.base();
                var self = this;
                self.title("Perfil");
                var form = this.newDataForm();
                form.addTextBox({ caption: "Login", readOnly: true, id: "login", value: Binding("id") });
                this.name = form.newTextBox({ caption: "Nome", id: "name", value: Binding("name"), required: true, hintText: "nome completo" });
                form.addEmailBox({ caption: "Email", id: "email", value: Binding("email"), required: true, hintText: "email de contato" });
                form.addSection({ caption: "Alterar senha" });
                form.addPasswordBox({ caption: "Senha atual", id: "currentPwd", hintText: "digite sua senha atual" });
                form.addPasswordBox({ caption: "Nova senha", id: "newPwd", hintText: "digite a nova senha" });
                form.addPasswordBox({ caption: "Confirme a nova senha", id: "newPwdCheck", hintText: "confirme a nova senha" });

                this.addDialogButton({ caption: "Ok", onClick: function ()
                {
                    form.clearErrors();
                    if (form.validate())
                    {
                        // comparar as senhas
                        var pwd1 = form.getFieldValue("newPwd");
                        var pwd2 = form.getFieldValue("newPwdCheck");
                        var curr = form.getFieldValue("currentPwd");

                        if (!Util.isEmpty(pwd1) || !Util.isEmpty(pwd2))
                        {
                            if (form.getControl("currentPwd").validateRequired())
                            {
                                if (pwd1 != pwd2)
                                {
                                    form.setFieldError("pwd1", "O campo senha e confirmação não conferem");
                                    return;
                                }
                            }
                        }
                        Net.exec("user.saveProfile", {
                            login: form.getFieldValue("login"),
                            name: form.getFieldValue("name"),
                            email: form.getFieldValue("email")
                        }, function (result)
                        {
                            if (result.success)
                            {
                                Shell.statusMessage("Perfil alterado com sucesso");
                                if (!Util.isEmpty(pwd1))
                                {
                                    Net.rpc("user.changePassword", { current: curr, newPwd: pwd1 }, function ()
                                    {
                                        self.close();
                                    });
                                }
                                else
                                {
                                    self.close();
                                }
                            }
                            else
                                form.setErrors(result.errors);
                        });
                    }
                }
                });
                this.addDialogButton({ caption: "Cancelar", onClick: function ()
                {
                    self.disposeOnClose(true);
                    self.close();
                }
                });
            },
            open: function ()
            {
                this.base(true);
                this.name.setFocus();
            }
        },
        properties: {
            user: {
                get: function () { return this._user; },
                set: function (value) { this._user = value; this.dataBind(value); }
            }
        }
    });



    //shell bar
    var shellBar = Shell.getMainBar();

    // botão do usuário.
    var userBtn = new wui.Button({ caption: User.name(), dropDownMenuGroup: "current_user", commandParam: null });
    shellBar.createGroup("curr_user", true);
    shellBar.add(userBtn, "curr_user");
    shellBar.add(new wui.Button({ command: "user.logout" }), "curr_user");


});