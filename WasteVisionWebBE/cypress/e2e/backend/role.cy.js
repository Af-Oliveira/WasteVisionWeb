// cypress/e2e/role.cy.js

describe("Role API E2E Tests", () => {
  // --- SETUP ---
  // Base URL for the Role API
  const roleBaseUrl = "http://localhost:3000/api/role"; // Adjusted port

  // This variable will be shared across all tests in this suite
  let roleId;

  // --- CREATE (POST) ---
  it("should create a new role and return it", () => {
    const testRole = {
      description: `E2E Test Role ${Date.now()}`,
    };

    cy.request({
      method: "POST",
      url: roleBaseUrl,
      body: testRole,
    }).then((response) => {
      // NOTE: Based on your C# code, the data is in `response.body`.
      // Based on your user.cy.js example, it's `response.body`.
      // I am following your user.cy.js example's pattern.
      const role = response.body;

      expect(response.status).to.eq(200);
      expect(role).to.have.property("description", testRole.description);
      expect(role).to.have.property("id").and.to.be.a("string");

      // Store the role's ID for subsequent tests
      roleId = role.id;
      cy.log(`Role created successfully with ID: ${roleId}`);
    });
  });

  it("should reject role creation with an empty description", () => {
    const invalidRole = {
      description: "",
    };

    cy.request({
      method: "POST",
      url: roleBaseUrl,
      body: invalidRole,
      failOnStatusCode: false, // Expecting a non-2xx status
    }).then((response) => {
      expect(response.status).to.eq(400);
    });
  });

  // --- READ (GET) ---
  it("should get a role by its ID", () => {
    expect(roleId, "roleId must be set by the creation test").to.not.be
      .undefined;

    cy.request("GET", `${roleBaseUrl}/${roleId}`).then((response) => {
      expect(response.status).to.eq(200);
      const role = response.body;
      expect(role).to.have.property("id", roleId);
    });
  });

  // --- UPDATE (PUT) ---
  it("should update an existing role's details", () => {
    expect(roleId).to.not.be.undefined;

    const updatedRoleDetails = {
      id: roleId,
      description: `Updated Role ${Date.now()}`,
      active: true,
    };

    cy.request({
      method: "PUT",
      url: `${roleBaseUrl}/${roleId}`,
      body: updatedRoleDetails,
    }).then((response) => {
      expect(response.status).to.eq(200);
      const role = response.body;
      expect(role).to.have.property(
        "description",
        updatedRoleDetails.description
      );
    });
  });

  // --- DEACTIVATE (SOFT DELETE) ---
  it("should deactivate (soft delete) a role", () => {
    expect(roleId).to.not.be.undefined;

    cy.request("DELETE", `${roleBaseUrl}/soft/${roleId}`).then((response) => {
      expect(response.status).to.eq(200);
      expect(response.body).to.be.true;
    });
  });

  // --- ACTIVATE (PATCH) ---
  it("should activate a deactivated role", () => {
    expect(roleId).to.not.be.undefined;

    cy.request("PATCH", `${roleBaseUrl}/activate/${roleId}`).then(
      (response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.true;
      }
    );
  });

  // --- `after()` HOOK FOR CLEANUP ---
  // This runs ONCE after all tests in this 'describe' block are complete.
  // We clean up the Role to keep the database clean.
  after(() => {
    if (roleId) {
      cy.log(`Cleaning up Role with ID: ${roleId}`);
      cy.request({
        method: "DELETE",
        // Using the 'hard' delete endpoint from your RoleController
        url: `${roleBaseUrl}/hard/${roleId}`,
        failOnStatusCode: false, // Don't fail the test run if cleanup fails
      });
    }
  });
});