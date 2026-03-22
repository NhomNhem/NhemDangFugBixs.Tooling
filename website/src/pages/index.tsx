import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import Heading from '@theme/Heading';

import styles from './index.module.css';

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={clsx('hero hero--primary', styles.heroBanner)}>
      <div className="container">
        <Heading as="h1" className="hero__title" style={{fontWeight: 800, fontSize: '2.8rem', letterSpacing: '-1px'}}>
          NhemDangFugBixs Tooling
        </Heading>
        <p className="hero__subtitle" style={{fontSize: '1.25rem', color: '#e0e7ff', fontWeight: 500, marginBottom: 24}}>
          A Roslyn Source Generator & Analyzer toolkit for Unity, automating DI with VContainer.
        </p>
        <div className={styles.buttons}>
          <Link
            className="button button--secondary button--lg"
            style={{background: '#6366f1', color: '#fff', fontWeight: 600, fontSize: '1.1rem', borderRadius: 8, boxShadow: '0 2px 8px #0002'}} 
            to="/docs/intro">
            View Documentation
          </Link>
        </div>
      </div>
    </header>
  );
}

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={`Hello from ${siteConfig.title}`}
      description="Description will go into a meta tag in <head />">
      <HomepageHeader />
      <main>
        <div style={{textAlign: 'center', margin: '2rem 0', background: 'linear-gradient(90deg, #e0e7ff 0%, #f0fdfa 100%)', borderRadius: 16, boxShadow: '0 4px 24px #0001', padding: '2.5rem 1rem'}}>
  <h2 className={styles.keyFeatureTitle}>
  <span className={styles.keyFeatureStar}></span>
  Key Features
</h2>
  <ul className={styles.featureList}>
    <li className={styles.featureItem}><span className={styles.iconCheck}></span><b>Auto-generates DI registration</b> for VContainer</li>
    <li className={styles.featureItem}><span className={styles.iconCheck}></span><b>Compile-time DI checks</b> for early error detection</li>
    <li className={styles.featureItem}><span className={styles.iconCheck}></span><b>Customizable attributes</b> & multiple scopes</li>
    <li className={styles.featureItem}><span className={styles.iconCheck}></span><b>Easy Unity integration</b> and fast onboarding</li>
  </ul>
  <div className={styles.featureActions}>
  <span style={{background: '#6366f1', color: '#fff', borderRadius: 8, padding: '0.5em 1.5em', fontWeight: 500, fontSize: '1.1rem', boxShadow: '0 2px 8px #0001'}}>
    <Link to="/docs/intro" style={{color: '#fff', textDecoration: 'none'}}>Get Started</Link>
  </span>
  <span className={styles.orText}>or</span>
  <Link to="/docs/architecture" style={{color: '#2563eb', fontWeight: 600, fontSize: '1.1rem'}}>See Architecture</Link>
</div>
</div>
      </main>
    </Layout>
  );
}
