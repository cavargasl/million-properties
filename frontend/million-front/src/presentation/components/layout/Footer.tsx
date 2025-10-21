import Link from 'next/link';
import { Building2, Github, Linkedin } from 'lucide-react';
import { NAVIGATION_LINKS, SITE_CONFIG } from '@/shared/constants/navigation';

export function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="border-t border-border bg-background">
      <div className="container max-w-screen-2xl mx-auto px-4 sm:px-8 py-8">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Brand */}
          <div className="flex flex-col gap-4">
            <Link href="/" className="flex items-center gap-2 font-bold text-xl">
              <Building2 className="h-6 w-6 text-primary" />
              <span>{SITE_CONFIG.name}</span>
            </Link>
            <p className="text-sm text-muted-foreground">
              {SITE_CONFIG.description}
            </p>
          </div>

          {/* Navegación */}
          <div>
            <h3 className="font-semibold mb-4">Navegación</h3>
            <ul className="space-y-2 text-sm text-muted-foreground">
              {
                NAVIGATION_LINKS.map(({ href, name }) => (
                  <li key={href}>
                    <Link href={href} className="hover:text-primary transition-colors">
                      {name}
                    </Link>
                  </li>
                ))
              }
            </ul>
          </div>

          {/* Recursos */}
          <div>
            <h3 className="font-semibold mb-4">Recursos</h3>
            <ul className="space-y-2 text-sm text-muted-foreground">
              <li>
                <Link href="#" className="hover:text-primary transition-colors">
                  Documentación
                </Link>
              </li>
              <li>
                <Link href="#" className="hover:text-primary transition-colors">
                  API
                </Link>
              </li>
              <li>
                <Link href="#" className="hover:text-primary transition-colors">
                  Soporte
                </Link>
              </li>
            </ul>
          </div>

          {/* Contacto */}
          <div>
            <h3 className="font-semibold mb-4">Contacto</h3>
            <div className="flex gap-4">
              <a
                href="https://github.com/cavargasl/million-properties"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="GitHub"
              >
                <Github className="h-5 w-5" />
              </a>
              <a
                href="https://www.linkedin.com/in/cavargasl/"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="LinkedIn"
              >
                <Linkedin className="h-5 w-5" />
              </a>
            </div>
          </div>
        </div>

        <div className="mt-8 pt-8 border-t border-border">
          <p className="text-center text-sm text-muted-foreground">
            © {currentYear} {SITE_CONFIG.name}. Todos los derechos reservados.
          </p>
        </div>
      </div>
    </footer>
  );
}
