import { HeroSection } from "@/presentation/components/ui";

export default function Home() {
  return (
    <div className="flex flex-col min-h-[calc(100vh-4rem)]">
      <main className="flex-1">
        <div className="container max-w-screen-2xl mx-auto">
          <HeroSection />
        </div>
      </main>
    </div>
  );
}
