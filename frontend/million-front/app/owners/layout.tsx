import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Propietarios",
  description: "Gestiona los propietarios de las propiedades",
};

export default function OwnersLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <>{children}</>;
}
