import { IntervalHistogram } from "perf_hooks";

export function LiveResertLogin(IDPassport: string): boolean {
    cy.request({
        method:'POST', 
        url:'http://localhost:7068/api/LiveResertLogin',
     
        body: {
          username: 'jane.lane',
          password: 'password123',
        },
        qs: {
          IDPassport:IDPassport
        },
        
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('isSuccess', 'true');
      });
    return true
}

export function InvalidAccessAttempsByIDPassport(IDPassport: string): number {
    cy.request({
        method:'GET', 
        url:'http://localhost:7068/api/GETInvalidAccessAttempsByIDPassport',
        qs: {
          IDPassport:IDPassport
        },
        
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('isSuccess', 'true');
        return response.body.InvalidAccessAttemps
      });
    return 99
}

