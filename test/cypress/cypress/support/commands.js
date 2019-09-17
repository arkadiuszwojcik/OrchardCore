import { creds } from './objects';

// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add("login", (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add("drag", { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add("dismiss", { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This is will overwrite an existing command --
// Cypress.Commands.overwrite("visit", (originalFn, url, options) => { ... })

Cypress.Commands.add('login', function({ prefix = '' } = {}) {
    cy.visit(`${prefix}/login`);
    cy.get('#UserName').type(creds.username);
    cy.get('#Password').type(creds.password);
    cy.get('form').submit();
});

Cypress.Commands.add('gotoTenantSetup', ({ name }) => {
    cy.visit('/OrchardCore.Tenants/Admin/Index');
    cy.get(`#btn-setup-${name}`).click();
});
Cypress.Commands.add('setupSite', ({ name, setupRecipe, username, email, password, passwordConfirmation }) => {
    cy.get('#SiteName').type(name);
    cy.get('body').then($body => {
        const elem = $body.find('#RecipeName');
        if (elem) {
            elem.val(setupRecipe);
        }
        const db = $body.find('#DatabaseProvider');
        if (db) {
            db.val('Sqlite');
        }
    });
    cy.get('#UserName').type(username);
    cy.get('#Email').type(email);
    cy.get('#Password').type(password);
    cy.get('#PasswordConfirmation').type(passwordConfirmation);
    cy.get('#SubmitButton').click();
});
Cypress.Commands.add('createTenant', ({ name, prefix }) => {
    // We create tenants on the SaaS tenant
    cy.visit('/OrchardCore.Tenants/Admin/Index');
    cy.get('#btn-add-tenant').click();
    cy.get('#Name').type(name);
    cy.get('#RequestUrlPrefix').type(prefix);
    cy.get('#btn-create').click();
});

Cypress.Commands.add('runRecipe', ({ prefix }, recipeName) => {
    cy.visit(`${prefix}/OrchardCore.Recipes/Admin/Index`);
    cy.get(`[data-run-button="${recipeName}"]`).click();
    cy.get('#modalOkButton').click();
});
Cypress.Commands.add('visitAdmin', ({ prefix }) => {
    cy.visit(`${prefix}/admin`);
});
Cypress.Commands.add('visitTenantPage', ({ prefix }, url) => {
    cy.visit(`${prefix}/${url}`);
});
