"use client";

import Link from "next/link";
import { useState } from "react";

const NAV_ITEMS = [
  { id: "hero", label: "Home" },
  { id: "about", label: "Sobre" },
  { id: "laboratory", label: "Laboratório" },
  { id: "services", label: "Serviços" },
  { id: "contacts", label: "Contato" },
];

export function Navbar() {
  const [open, setOpen] = useState(false);

  const handleNavClick = () => {
    setOpen(false);
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light fixed-top" id="mainNav">
      <div className="container-fluid px-4 px-lg-5">
        <Link className="navbar-brand" href="/">
          iribeiro.tec.br
        </Link>

        <button
          className="navbar-toggler"
          type="button"
          onClick={() => setOpen((o) => !o)}
          aria-controls="navbarResponsive"
          aria-expanded={open}
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div
          className={`collapse navbar-collapse ${open ? "show" : ""}`}
          id="navbarResponsive"
        >
          <ul className="navbar-nav ms-auto my-2 my-lg-0">
            {NAV_ITEMS.map((item) => (
              <li className="nav-item" key={item.id}>
                <a
                  className="nav-link"
                  href={`#${item.id}`}
                  onClick={handleNavClick}
                >
                  {item.label}
                </a>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </nav>
  );
}
