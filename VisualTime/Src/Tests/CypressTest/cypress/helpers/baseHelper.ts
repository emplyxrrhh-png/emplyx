export const retriesDefault: Partial<Cypress.ResolvedConfigOptions> = {
    retries: 0
}

export const createEmptyVisitOptions = (): Partial<Cypress.VisitOptions> => {
    const visitOptions: Partial<Cypress.VisitOptions> = {}
    return visitOptions
}

export function getDomElement(element: string | Cypress.Chainable<JQuery<HTMLElement>>, scrollIntoView?: boolean, waitForVisible?: boolean): Cypress.Chainable<JQuery<HTMLElement>> {
    let chainable: Cypress.Chainable<JQuery<HTMLElement>>
    if (typeof element === "string") {
        chainable = cy.get(element)
    } else {
        chainable = element
    }
    if (scrollIntoView) {
        chainable = chainable.scrollIntoView()
    }
    if (waitForVisible) {
        chainable = chainable.should("be.visible", { timeout: 30000, interval: 1000 })
    }
    return chainable
}

export function normalizeHtml(html: string): string {
    return html
        .replace(/\s+/g, " ") // Reduce espacios múltiples a un solo espacio
        .replace(/id=".*?"/g, "") //Elimina IDs dinámicos
        .replace(/data-cy=".*?"/g, "") // Elimina atributos 'data-cy' usados en pruebas
        .replace(/data-testid=".*?"/g, "") // Elimina atributos 'data-testid' usados en pruebas
        .replace(/unit-test-id=".*?"/g, "") // Elimina atributos 'unit-test-id'
        .replace(/<!--.*?-->/g, "") //Elimina comentarios HTML
        .replace(/aria-activedescendant=".*?"/g, "") //Elimina atributos 'aria-activedescendant'
        .trim()
}
