import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "react-router-dom";

export function DataProtectionPage() {
  return (
    <div className="container mx-auto py-8 px-4 max-w-6xl space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl font-bold">
            Data Protection Statement
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          <section className="space-y-4">
            <h2 className="text-xl font-semibold">1. General Information</h2>
            <div className="space-y-2">
              <p>
                <b>WasteVision.ai</b> is committed to safeguarding personal data
                and ensuring compliance with the{" "}
                <b>General Data Protection Regulation (GDPR)</b> and Portuguese
                law, specifically the{" "}
                <b>Sistema Nacional de Proteção de Dados (SNPD)</b>.
              </p>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">
              2. Key Data Protection Practices
            </h2>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                2.1 Data Controllers and Processors
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  Data is processed internally by authorized personnel and
                  externally by trusted third parties (e.g., Google
                  Authentication via Keycloak).
                </li>
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                2.2 Legal Bases for Processing
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  We process data under legal bases such as user consent,
                  contractual necessity, vital interest, and legitimate
                  interest.
                </li>
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                2.3 Data Security and Storage
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  Data is securely stored on <b>Azure Cloud</b> and{" "}
                  <b>Linux servers</b>, with encryption and regular backups.
                </li>
                <li>
                  Access is restricted to authorized personnel, ensuring
                  confidentiality and integrity.
                </li>
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">2.4 Data Subject Rights</h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  We respect user rights, including access, correction, erasure,
                  and portability, as mandated by GDPR.
                </li>
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">2.5 Data Retention</h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  Retention aligns with legal requirements.
                </li>
              </ul>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">3. Supervisory Authority</h2>
            <div className="space-y-2">
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  For unresolved complaints, you may contact the <b>SNPD</b> or
                  your local data protection authority.
                </li>
              </ul>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">4. Contact Information</h2>
            <div className="space-y-2">
              <ul className="list-disc pl-6 space-y-1">
                <li>
                  <b>Data Protection Officer</b>: Gilmário Baltazar
                </li>
                <li>
                  <b>Email</b>: data.protection@WasteVision.ai
                </li>
                <li>
                  <b>Phone</b>: +351 912435846
                </li>
              </ul>
            </div>
          </section>

          <section className="mt-6 text-sm text-muted-foreground">
            <p>Last updated: {new Date().toLocaleDateString()}</p>
            <p>
              This statement is a high-level summary of our data protection
              practices. For detailed information, refer to our{" "}
              <Link
                to="/privacy-policy"
                className="text-blue-600 hover:underline"
              >
                Privacy Policy
              </Link>
              .
            </p>
          </section>
        </CardContent>
      </Card>
    </div>
  );
}
